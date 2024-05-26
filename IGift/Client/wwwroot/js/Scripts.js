function InitializeInactivityTimer(dotnetHelper) {
    var timer;
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        timer = setTimeout(Logout, 3000);
    }

    function Logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }

}