function Superset_IsChild(parent, child) {
    while (child !== undefined && child.nodeType !== 9) {
        if (child.isEqualNode(parent)) {
            return true;
        }
        child = child.parentNode;
    }
    return false;
}
