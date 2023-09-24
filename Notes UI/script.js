const saveButton = document.querySelector('#btnSave');
const titleInput = document.querySelector('#title');
const titleDescription = document.querySelector('#description');
const notesContainer = document.querySelector('#notes__container');
const deleteButton = document.querySelector('#btnDelete');

function clearForm(){
    titleInput.value = '';
    titleDescription.value = '';
    deleteButton.classList.add('hidden');
}

function displayNoteInForm(note){
    titleInput.value = note.title;
    titleDescription.value = note.description;
    deleteButton.classList.remove('hidden');
    deleteButton.setAttribute('data-id',note.id);
    saveButton.setAttribute('data-id',note.id);
}

function GetNoteById(id){
    fetch(`https://localhost:7041/api/notes/${id}`)
    .then(data => data.json())
    .then(response => displayNoteInForm(response));
}

function populateForm(id){
    GetNoteById(id);
}

function AddNote(title, description){
    const body = {
        title: title,
        description: description,
        isVisible: true
    };
    fetch('https://localhost:7041/api/notes/', {
        method: 'POST',
        body: JSON.stringify(body),
        headers: {
            "content-type": "application/json"
        }
    })
    .then(data => data.json())
    .then(response => {
        clearForm();
        GetAllNotes();
    });
}

function DisplayNotes(notes) {
    let allNotes = '';

    notes.forEach(note => {
    const noteElement = `
                    <div class="note" data-id="${note.id}">
                        <h3>${note.title}</h3>
                        <p>${note.description}</p>
                    </div> `;
    allNotes += noteElement;
    });

    notesContainer.innerHTML = allNotes; 
    
    //add event listerners  
    document.querySelectorAll('.note').forEach(note => {
        note.addEventListener('click', function(){
            populateForm(note.dataset.id);
        });
    });
}

function GetAllNotes(){
    fetch('https://localhost:7041/api/notes/')
    .then(data => data.json())
    .then(response => DisplayNotes(response));
}

GetAllNotes();

function UpdateNote(id, title, description){
    const body = {
        title: title,
        description: description,
        isVisible: true
    };

    fetch(`https://localhost:7041/api/notes/${id}`, {
        method: 'PUT',
        body: JSON.stringify(body),
        headers: {
            "content-type": "application/json"
        }
    })
    .then(data => data.json())
    .then(response => {
        clearForm();
        GetAllNotes();
    });
}

saveButton.addEventListener('click', function(){
    const id = saveButton.dataset.id;
    if (id){
        UpdateNote(id, titleInput.value, titleDescription.value);
    } else {
        AddNote(titleInput.value, titleDescription.value);
    }
});

function deleteNote(id){
    fetch(`https://localhost:7041/api/notes/${id}`, {
        method: 'DELETE',
        headers: {
            "content-type": "application/json"
        }
    })
    .then(response => {
        clearForm();
        GetAllNotes();
    });
}
deleteButton.addEventListener('click', function(){
    const id = deleteButton.dataset.id;
    deleteNote(id);
});