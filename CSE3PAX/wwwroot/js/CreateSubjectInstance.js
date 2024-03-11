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

            // Update the read-only input fields for lecturer's information
            document.getElementById('lecturerEmail').value = email;
            document.getElementById('lecturerFirstName').value = row.cells[0].textContent;
            document.getElementById('lecturerLastName').value = row.cells[1].textContent;

            // Hide the lecturer table again
            document.getElementById('lecturerTable').style.display = 'none';
        }
    });

    // Event listener for the start date picker
    document.getElementById('startDate').addEventListener('change', function () {
        const startDate = new Date(this.value);
        const endDate = new Date(startDate);

        // Add 84 days to the start date to get the end date
        endDate.setDate(startDate.getDate() + 84);

        // Update the end date picker
        // Ensuring the date is in the correct format (YYYY-MM-DD) for the input
        document.getElementById('endDate').value = endDate.toISOString().split('T')[0];
    });
});
