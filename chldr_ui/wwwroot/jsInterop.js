// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

// Создаем функцию для открытия модального окна
function openModal() {
    // Создаем элементы модального окна
    var modal = document.createElement('div');
    modal.className = 'modal';

    var content = document.createElement('div');
    content.className = 'modal-content';

    var closeButton = document.createElement('button');
    closeButton.className = 'close-button';
    closeButton.innerHTML = 'Закрыть';

    // Добавляем обработчик события для закрытия модального окна при клике на кнопку "Закрыть"
    closeButton.addEventListener('click', closeModal);

    // Добавляем содержимое модального окна
    content.appendChild(closeButton);
    modal.appendChild(content);

    // Добавляем модальное окно в тело документа
    document.body.appendChild(modal);
}

// Создаем функцию для закрытия модального окна
function closeModal() {
    var modal = document.querySelector('.modal');
    document.body.removeChild(modal);
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