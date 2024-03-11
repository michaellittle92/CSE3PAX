document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('selectLecturerBtn').addEventListener('click', function () {
        // Show the lecturer table
        console.log('Select Lecturer button clicked');
        document.getElementById('lecturerTable').style.display = '';
    });

    document.getElementById('lecturerTable').addEventListener('click', function (e) {
        // Check if the clicked element is a "Select" button
        if (e.target && e.target.nodeName == "BUTTON" && e.target.classList.contains('selectLecturer')) {
            const row = e.target.closest('tr');
            const email = row.cells[2].textContent; 

            // Update the read-only input field for lecturer's email
            document.getElementById('lecturerEmail').value = email;

            document.getElementById('lecturerFirstName').value = row.cells[0].textContent; 
            document.getElementById('lecturerLastName').value = row.cells[1].textContent; 

            // Hide the lecturer table again
            document.getElementById('lecturerTable').style.display = 'none';
        }
    });
});
