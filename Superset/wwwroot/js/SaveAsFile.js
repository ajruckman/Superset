// https://stackoverflow.com/a/53822526/9911189
window.Superset_SaveAsFile = function (filename, bytesBase64) {
    const link = document.createElement("a");
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
};
