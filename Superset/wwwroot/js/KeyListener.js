window.Superset_AddKeyCallback = function (target, dotnetHelper) {
    console.log(target);
    window.Superset_KeyListeners[Math.random().toString(36).substring(2, 15)] = {
        target: target,
        dotnetHelper: dotnetHelper,
    };
};

if (window.Superset_KeyListeners == null) {
    window.Superset_KeyListeners = [];

    document.onkeyup = function (e) {
        for (let key in window.Superset_KeyListeners) {
            if (!window.Superset_KeyListeners.hasOwnProperty(key)) {
                continue;
            }

            if (!(e.target instanceof Element)) {
                console.log(e.target, "is not an Element");
                continue;
            }

            let listener = window.Superset_KeyListeners[key];

            console.log(e);
            console.log(key);
            console.log(key.target);

            if (listener.target.isEqualNode(e.target)) {
                listener.dotnetHelper.invokeMethodAsync("KeyUp", e.key, e.target.id, e.shiftKey, e.ctrlKey);

            } else if (Superset_IsChild(listener.target, e.target)) {
                listener.dotnetHelper.invokeMethodAsync("InnerKeyUp", e.key, e.target.id, e.shiftKey, e.ctrlKey);

            } else {
                listener.dotnetHelper.invokeMethodAsync("OuterKeyUp", e.key, e.target.id, e.shiftKey, e.ctrlKey);
            }
        }
    };
}
