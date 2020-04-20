window.Superset_InitTooltip = function (target, text) {
    console.log(target);
    tippy(target, {
        content: text,
        distance: 3,
        theme: 'superset',
    });
};
