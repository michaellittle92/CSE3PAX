document.addEventListener('DOMContentLoaded', function () {

    console.log("Testing Javascript");

    // Variables
    var ctx = document.getElementById('availabilityChart').getContext('2d');
    var available = 62;
    var unavailable = 38;

    // Data for doughnut chart
    var data = {
        labels: ['Available', 'Unavailable'],
        datasets: [{
            label: 'Staff Availability',
            data: [available, unavailable], // Availability percentage
            backgroundColor: ['#41B8D5', '#6CE5E8'],
            hoverBackgroundColor: ['#289BB6', '#20CACE']
        }]
    };

    // Configuration options
    var options = {
        maintainAspectRatio: false,
        responsive: true
    };

    // Create the doughnut chart
    var doughnut = new Chart(ctx, {
        type: 'doughnut',
        data: data,
        options: options
    });
});