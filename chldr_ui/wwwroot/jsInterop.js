// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function enable(selector) {
    var container = document.querySelector(selector);
    var elements = container.getElementsByTagName("*");

    for (var i = 0; i < elements.length; i++) {
        elements[i].disabled = false;
    }
}
export function disable(selector) {
    var container = document.querySelector(selector);
    var elements = container.getElementsByTagName("*");

    for (var i = 0; i < elements.length; i++) {
        elements[i].disabled = true;
    }
}

export function showConfirmationDialog() {
    // Пример использования:
    openModal();
}

export function showPrompt(message) {
    return prompt(message, 'Type anything here');
}

export function clickShowRandoms() {
    try {
        const btnShowRandoms = document.querySelector(".chldr_btn_show-randoms");
        btnShowRandoms.click(), 3000
    } catch (ex) {
        console.log("couldn't click it :(");
    }
}