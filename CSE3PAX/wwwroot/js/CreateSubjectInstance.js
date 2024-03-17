document.querySelectorAll('.select-lecturer').forEach(button => {
    button.addEventListener('click', function () {
        // Retrieve data attributes from the clicked button
        const email = this.getAttribute('data-email');
        const firstName = this.getAttribute('data-firstname');
        const lastName = this.getAttribute('data-lastname');

        // Update the readonly input fields
        document.getElementById('selectedEmail').value = email;
        document.getElementById('selectedFirstName').value = firstName;
        document.getElementById('selectedLastName').value = lastName;

        const hiddenEmailInput = document.querySelector('input[name="SelectedEmail"]');
        if (hiddenEmailInput) {
            hiddenEmailInput.value = email;
        }
    });
});
