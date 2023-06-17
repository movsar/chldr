let mediaRecorder;
let chunks = [];
let recordedAudio;

const recordButton = document.getElementById("recordButton");

export function toggleRecording() {
    if (isRecording()) {
        stopRecording();
        showPlaybackControls();
    } else {
        startRecording();
        showStopButton();
    }
}

export function isRecording() {
    return mediaRecorder && mediaRecorder.state === "recording";
}

export function startRecording() {
    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(function (stream) {
            chunks = [];
            mediaRecorder = new MediaRecorder(stream);

            mediaRecorder.addEventListener("dataavailable", function (event) {
                chunks.push(event.data);
            });

            mediaRecorder.addEventListener("stop", function () {
                const audioBlob = new Blob(chunks, { type: "audio/wav" });
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

export function createRemoveButton() {
    // Create a button element
    var button = document.createElement('button');

    // Add classes to the button for styling
    button.classList.add('btn');
    button.classList.add('btn-danger');

    // Set the text of the button to "Remove"
    button.textContent = 'Remove';

    // Add an event listener to handle the button click
    button.addEventListener('click', function () {
        // Remove the button when clicked
        button.parentNode.removeChild(button);
    });


    // // Create a span element for the remove icon
    // var icon = document.createElement('span');

    // // Add a class to the icon for styling
    // icon.classList.add('remove-icon');

    // // Append the icon to the button
    // button.appendChild(icon);

    // Append the button to the document body or any other desired element

    return button;
}

export function stopRecording() {
    if (isRecording()) {
        mediaRecorder.stop();
    }
}

export function showStopButton() {
    recordButton.textContent = "Stop";
    recordButton.classList.remove("play");
    recordButton.classList.add("stop");
}

export function showPlaybackControls() {
    recordButton.textContent = "Record";
    recordButton.classList.remove("stop");
    recordButton.classList.add("Record");
}


export function handleSaveClick() {
    resetRecorder();
}

export function resetRecorder() {
    stopRecording();

    recordButton.textContent = "Record";
    recordButton.classList.remove("stop", "play");
    recordButton.classList.add("record");
    document.getElementById("audioContainer").innerHTML = "";
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