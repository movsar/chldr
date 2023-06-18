let mediaRecorder;
let chunks = [];
let recordedBlob;
let recordedAudio;

const recordButton = document.getElementById("recordButton");
export function startRecording() {
    showStopButton();

    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(function (stream) {
            chunks = [];
            mediaRecorder = new MediaRecorder(stream);
            mediaRecorder.audioBitsPerSecond
            mediaRecorder.addEventListener("dataavailable", function (event) {
                chunks.push(event.data);
            });

            mediaRecorder.addEventListener("stop", function () {
                const audioBlob = new Blob(chunks, { type: "audio/ogg; codecs=opus" });
                recordedBlob = audioBlob;

                const audioUrl = URL.createObjectURL(audioBlob);
                recordedAudio = new Audio(audioUrl);
                recordedAudio.controls = true;

                let removeButton = createRemoveButton();

                document.getElementById("audioContainer").appendChild(recordedAudio);
                document.getElementById("audioContainer").appendChild(removeButton);
            });

            mediaRecorder.start();
        })
        .catch(function (err) {
            console.log("The following error occurred: " + err);
        });
}

export function stopRecording() {
    showStartButton();

    return new Promise((resolve, reject) => {
        mediaRecorder.addEventListener("stop", function () {
            const reader = new FileReader();
            reader.onloadend = function () {
                const base64String = reader.result.split(',')[1];
                resolve(base64String);
            };
            reader.readAsDataURL(recordedBlob);
        });
        mediaRecorder.stop();
    });
}

export function showStopButton() {
    recordButton.textContent = "Stop recording";
    recordButton.classList.remove("record");
    recordButton.classList.add("stop");
}

export function showStartButton() {
    recordButton.textContent = "Add pronunciation";
    recordButton.classList.remove("stop");
    recordButton.classList.add("record");
}

export function createRemoveButton() {
    // Create a button element
    var button = document.createElement('button');

    // Add classes to the button for styling
    button.classList.add('btn');
    button.classList.add('btn-danger');

    // Set the text of the button to "Remove"
    button.textContent = 'Remove';

    return button;
}
export function enable(selector) {
    var container = document.querySelector(selector);
    if (container == null) {
        return;
    }
    var elements = container.getElementsByTagName("*");

    for (var i = 0; i < elements.length; i++) {
        elements[i].disabled = false;
    }
}
export function disable(selector) {
    var container = document.querySelector(selector);
    if (container == null) {
        return;
    }
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