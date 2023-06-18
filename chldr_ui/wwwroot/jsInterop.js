﻿let mediaRecorder;
let chunks = [];
let recordedBlob;
let latestRecordingId;

const recordButton = document.getElementById("recordButton");
const audioContainer = document.getElementById("audioContainer");

export function startRecording(recordingId) {
    showStopButton();

    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(function (stream) {

            latestRecordingId = recordingId;

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
                const recordedAudio = createAudioElement(audioUrl, recordingId);

                let removeButton = createRemoveButton(recordingId);

                audioContainer.appendChild(recordedAudio);
                audioContainer.appendChild(removeButton);
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
                resolve({ id: latestRecordingId, data: base64String });
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

export function createAudioElement(audioUrl, recordingId) {
    const recordedAudio = document.createElement("audio");
    recordedAudio.src = audioUrl;
    recordedAudio.controls = true;
    recordedAudio.id = `recordedAudio_${recordingId}`;

    return recordedAudio;
}

export function createRemoveButton(recordingId) {
    var button = document.createElement('button');
    button.classList.add('btn');
    button.classList.add('btn-danger');
    button.textContent = 'Remove';
    button.addEventListener('click', () => {
        const audioToRemove = document.getElementById(`recordedAudio_${recordingId}`);
        audioToRemove.remove();
        button.remove();
         
        DotNet.invokeMethodAsync("chldr_shared", "WordEdit_RemoveSound_ClickHandler", recordingId);
    });

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