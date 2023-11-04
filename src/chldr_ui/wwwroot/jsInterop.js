let mediaRecorder;
let recordedBlob;

let _lblRecord;
let _lblStopRecording;
let _latestRecordingId;
export function addRecordingListItem(id, data, canPromote, canRemove) {
    const audioContainer = document.querySelector("#recordings-list");
    if (!audioContainer) {
        console.error("audio container not found");
        return;
    }

    const audioUrl = URL.createObjectURL(base64ToBlob(data));
    const recordedAudio = createAudioElement(audioUrl, id);

    const listItem = document.createElement("div");
    listItem.classList.add("recording-list-item");
    listItem.id = `rli_${id}`;
    listItem.appendChild(recordedAudio);

    if (canPromote) {
        const promoteButton = createPromoteButton(id);
        listItem.appendChild(promoteButton);
    }

    if (canRemove) {
        const removeButton = createRemoveButton(id);
        listItem.appendChild(removeButton);
    }

    audioContainer.appendChild(listItem);
}

function base64ToBlob(base64String) {
    const byteCharacters = atob(base64String);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    const byteArray = new Uint8Array(byteNumbers);

    return new Blob([byteArray], { type: "audio/ogg; codecs=opus" });
}
export function addExistingEntryRecording(soundDto) {
    console.log(soundDto);

    addRecordingListItem(soundDto.soundId, base64ToBlob(soundDto.recordingB64));
}

export function startRecording(recordingId, lblRecord, lblStopRecording) {
    _lblRecord = lblRecord;
    _lblStopRecording = lblStopRecording;

    showStopButton();

    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(function (stream) {

            _latestRecordingId = recordingId;

            let chunks = [];

            mediaRecorder = new MediaRecorder(stream);
            mediaRecorder.audioBitsPerSecond
            mediaRecorder.addEventListener("dataavailable", function (event) {
                chunks.push(event.data);
            });

            mediaRecorder.addEventListener("stop", function () {
                const audioBlob = new Blob(chunks, { type: "audio/ogg; codecs=opus" });
                recordedBlob = audioBlob;
            });

            mediaRecorder.start();
        })
        .catch(function (err) {
            console.log("The following error occurred: " + err);
        });
}

export function stopRecording(entryId) {
    if (!mediaRecorder) {
        return;
    }

    let result = new Promise((resolve, reject) => {
        mediaRecorder.addEventListener("stop", function () {
            const reader = new FileReader();
            reader.onloadend = function () {
                const base64String = reader.result.split(',')[1];
                resolve({ id: _latestRecordingId, data: base64String });
            };
            reader.readAsDataURL(recordedBlob);
        });
        mediaRecorder.stop();
    });

    AddSoundRequest(entryId, result.id, result.data);
}

function AddSoundRequest(entryId, soundId, soundBase64String) {
    const session = localStorage.getItem("session");

    const operation = "addSound";
    const requestBody = {
        query: `
            mutation ${operation}($entryId: String!, $soundId: String!,$soundBase64String: String!) {
                ${operation}(entryId: $entryId, soundId: $soundId, soundBase64String: $soundBase64String) {
                    success
                    errorMessage
                    serializedData
                }
            }
        `,
        variables: {
            entryId, soundId, soundBase64String
        }
    };

    const response = await fetch('YOUR_GRAPHQL_ENDPOINT_HERE', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${session}`
        },
        body: JSON.stringify(requestBody)
    });

    const responseData = await response.json();

    if (!response.ok) {
        throw new Error(`Error: ${responseData.errors[0].message}`);
    }

    return responseData.data;
}

export function showStopButton() {
    const recordButton = document.getElementById("recordButton");

    recordButton.textContent = _lblStopRecording;
    recordButton.classList.remove("record");
    recordButton.classList.add("stop");
}

export function showStartButton() {
    const recordButton = document.getElementById("recordButton");

    recordButton.textContent = _lblRecord;
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
    button.classList.add('remove-button');
    button.classList.add('bi');
    button.classList.add('bi-trash');
    button.addEventListener('click', () => {
        let recordingListItem = document.querySelector(`#rli_${recordingId}`);
        recordingListItem.remove();
        DotNet.invokeMethodAsync("chldr_shared", "WordEdit_RemoveSound_ClickHandler", recordingId);
    });

    return button;
}

export function createPromoteButton(recordingId) {
    var button = document.createElement('button');
    button.classList.add('promote-button');
    button.classList.add('bi');
    button.classList.add('bi-check-lg');
    button.addEventListener('click', () => {
        let recordingListItem = document.querySelector(`#rli_${recordingId}`);
        recordingListItem.remove();
        DotNet.invokeMethodAsync("chldr_shared", "WordEdit_PromoteSound_ClickHandler", recordingId);
    });

    return button;
}
export function addClass(selector, className) {
    var element = document.querySelector(selector);
    if (element == null) {
        return;
    }
    element.classList.add(className);
}
export function removeClass(selector, className) {
    var element = document.querySelector(selector);
    if (element == null) {
        return;
    }
    element.classList.remove(className);
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