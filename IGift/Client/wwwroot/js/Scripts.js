function InitializeInactivityTimer(dotnetHelper) {
    var timer;
    var timerMessage;
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        clearTimeout(timerMessage);
        timer = setTimeout(Logout, 90000);
        timerMessage = setTimeout(ShowMessage, 75000);
    }

    function Logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }

    function ShowMessage() {
        dotnetHelper.invokeMethodAsync("ShowMessage");
    }
}
