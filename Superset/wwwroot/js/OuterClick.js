if (window.Superset_OuterClickListeners == null) {
    window.Superset_OuterClickListeners = [];

    document.onmouseup = function (e) {
        for (let key in window.Superset_OuterClickListeners) {
            if (!window.Superset_OuterClickListeners.hasOwnProperty(key)) {
                continue;
            }

            if (!(e.target instanceof Element)) {
                console.log(e.target, "is not an Element");
                continue;
            }

            let listener = window.Superset_OuterClickListeners[key];

            console.log(key);
            console.log(listener);
            console.log(e);
            console.log(e.target.id);

            if (listener.target.isEqualNode(e.target)) {
                listener.dotnetHelper.invokeMethodAsync("Click", e.button, e.clientX, e.clientY, e.shiftKey, e.ctrlKey, e.target.id);
                
            } else if (isChild(listener.target, e.target)) {
                listener.dotnetHelper.invokeMethodAsync("InnerClick", e.button, e.clientX, e.clientY, e.shiftKey, e.ctrlKey, e.target.id);
                
            } else {
                listener.dotnetHelper.invokeMethodAsync("OuterClick", e.button, e.clientX, e.clientY, e.shiftKey, e.ctrlKey, e.target.id);
            }
        }
    };
}

function isChild(parent, child) {
    while (child !== undefined && child.nodeType !== 9) {
        if (child.isEqualNode(parent)) {
            return true;
        }
        child = child.parentNode;
    }
    return false;
}

window.clickCallback = function (id, dotnetHelper) {

    window.Superset_OuterClickListeners[id] = {
        target: document.getElementById(id),
        dotnetHelper: dotnetHelper,
    };

    // document.getElementById(id).addEventListener('outclick', function (e) {
    //     console.log('OuterClick')
    //     dotnetHelper.invokeMethodAsync('OuterClick');
    // });
};

// /**
//  * ALL CREDIT TO:
//  * Version: 0.1.0
//  * Author: Joseph Thomas
//  * Source: https://github.com/shade/outclick
//  */
// (function (window) {
//
//     const registeredIds = {};
//     const OutClickListeners = [{listener: null, exceptions: []}];
//
//     const addEventListener = Node.prototype.addEventListener;
//     const removeEventListener = Node.prototype.removeEventListener;
//
//     /** This handles any listener set by .onclick prototype property */
//     Object.defineProperty(Node.prototype, "onoutclick", {
//         set: function (func) {
//             OutClickListeners[0] = {
//                 exceptions: [this],
//                 listener: func && func.bind(this),
//             };
//
//             return func;
//         },
//     });
//
//     /** This handles all addEventListener */
//     window.Node.prototype.addEventListener = function (type, listener, exceptions) {
//         if (type === "outclick") {
//             let id = null;
//
//             // AJR 12-31-2019: Idk why this is here but it looks like a waste of performance and it works if I comment 
//             // it out, so I am going to disable it
//             // while (registeredIds[(id = (Math.random() * 100000).toString())]) {
//             // }
//             registeredIds[id] = listener;
//
//             exceptions = exceptions || [];
//             exceptions.push(this);
//             OutClickListeners.push({
//                 exceptions: exceptions,
//                 listener: listener && listener.bind(this),
//                 id: id,
//             });
//
//             return id;
//         } else {
//             addEventListener.apply(this, arguments);
//         }
//     };
//
//     window.document.addEventListener("click", function (e) {
//         for (let i = OutClickListeners.length; i--;) {
//             const listener = OutClickListeners[i];
//             let contains = false;
//
//             for (let g = listener.exceptions.length; g--;) {
//                 if (listener.exceptions[g].contains(e.target)) {
//                     contains = true;
//                     break;
//                 }
//             }
//
//             if (!contains) {
//                 listener.listener && listener.listener(e);
//             }
//         }
//     });
//
//     /** Getting rid of event listeners */
//     window.Node.prototype.removeEventListener = function (event, listener) {
//         if (event === "outclick") {
//             let i;
//             let id = -1;
//
//             if (typeof listener == "function") {
//                 for (i in registeredIds) {
//                     if (listener.toString() === registeredIds[i].toString()) {
//                         id = i;
//                         break;
//                     }
//                 }
//             } else {
//                 id = listener;
//             }
//             for (i = OutClickListeners.length; i--;) {
//                 const outListener = OutClickListeners[i];
//                 if (outListener.id === id) {
//                     OutClickListeners.splice(i, 1);
//                     break;
//                 }
//             }
//         } else {
//             removeEventListener.apply(this, arguments);
//         }
//     };
//
//     /** This handles the HTML onclick property */
//     const elements = document.querySelectorAll("[outclick]")
//
//     ;[].forEach.call(elements, function (e) {
//         const outclick = e.getAttribute("outclick");
//         const func = Function(outclick);
//         OutClickListeners.push({
//             listener: func,
//             exceptions: [e],
//         });
//     });
//
// })(window);