window.Superset_AddClickCallback = function (target, dotnetHelper) {
    window.Superset_ClickListeners[Math.random().toString(36).substring(2, 15)] = {
        target: target,
        dotnetHelper: dotnetHelper,
    };
};

if (window.Superset_ClickListeners == null) {
    window.Superset_ClickListeners = [];

    document.onmouseup = function (e) {
        for (let key in window.Superset_ClickListeners) {
            if (!window.Superset_ClickListeners.hasOwnProperty(key)) {
                continue;
            }

            if (!(e.target instanceof Element)) {
                console.log(e.target, "is not an Element");
                continue;
            }

            let listener = window.Superset_ClickListeners[key];

            if (listener.target.isEqualNode(e.target)) {
                listener.dotnetHelper.invokeMethodAsync("Click", e.button, e.target.id, e.clientX, e.clientY, e.shiftKey, e.ctrlKey);

            } else if (Superset_IsChild(listener.target, e.target)) {
                listener.dotnetHelper.invokeMethodAsync("InnerClick", e.button, e.target.id, e.clientX, e.clientY, e.shiftKey, e.ctrlKey);

            } else {
                listener.dotnetHelper.invokeMethodAsync("OuterClick", e.button, e.target.id, e.clientX, e.clientY, e.shiftKey, e.ctrlKey);
            }
        }
    };
}
