
//Desconectar al usuario despues de tiempo inactivo
function InitializeInactivityTimer(dotnetHelper) {
    var timer;
    var timerMessage;
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer);
        clearTimeout(timerMessage);
        timer = setTimeout(Logout, 180000);
        timerMessage = setTimeout(ShowMessage, 160000);
    }

    function Logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }

    function ShowMessage() {
        dotnetHelper.invokeMethodAsync("ShowMessage");
    }
}

//Escuchar la tecla Enter
window.chatInterop = {
    initializeEnterToSend: function (dotnetHelper) {
        console.log("chatInterop: inicializado");
        const input = document.getElementById("chatInput");
        if (!input) {
            console.warn("chatInterop: no se encontró el input");
            return;
        }

        input.addEventListener("keydown", function (e) {
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault();
                const message = input.value.trim();
                if (message !== "") {
                    dotnetHelper.invokeMethodAsync("SendMessageFromJs", message);
                    input.value = "";
                }
            }
        });
    }
};

//Scroll
window.chatInterop.scrollToBottom = function () {
    const container = document.getElementById("chatContainer");
    if (container) {
        container.scrollTop = container.scrollHeight;
    }
};

//Audio
window.PlayAudio = (elementName) => {
    document.getElementById(elementName).play();
}