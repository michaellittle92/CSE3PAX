document.addEventListener('DOMContentLoaded', function () {

    console.log("Testing Javascript");

    // Variables
    var ctx = document.getElementById('availabilityChart').getContext('2d');
    var lecturerNames = ['Lecturer 1', 'Lecturer 2', 'Lecturer 3', 'Lecturer 4', 'Lecturer 5', 'Lecturer 6', 'Lecturer 7', 'Lecturer 8', 'Lecturer 9', 'Lecturer 10', 'Lecturer 11', 'Lecturer 12'];
    // Hardcoded percentages to be updated with correct data
    var lecturerPercentages = [10, 20, 30, 40, 50, 60, 70, 80, 90, 15, 25, 35];

    // Data for doughnut chart
    var data = {
        labels: ['Available', 'Unavailable'],
        datasets: [{
            label: 'Staff Availability',
            data: [90, 10], // Initial data for available and unavailable
            backgroundColor: ['#41B8D5', '#6CE5E8'],
            hoverBackgroundColor: ['#289BB6', '#20CACE']
        }]
    };

    // Configuration options
    var options = {
        maintainAspectRatio: false,
        responsive: true,
        legend: {
            display: false
        },
        tooltips: {
            enabled: true,
            callbacks: {
                label: function (tooltipItem, data) {
                    var dataset = data.datasets[tooltipItem.datasetIndex];
                    var total = dataset.data.reduce(function (previousValue, currentValue, currentIndex, array) {
                        return previousValue + currentValue;
                    });
                    var currentValue = dataset.data[tooltipItem.index];
                    var percentage = Math.round((currentValue / total) * 100);
                    return data.labels[tooltipItem.index] + ': ' + percentage + '%';
                }
            }
        },
        plugins: {
            datalabels: {
                formatter: function (value, context) {
                    return context.chart.data.labels[context.dataIndex] + ': ' + value + '%';
                },
                color: '#fff'
            }
        }
    };

    // Create the doughnut chart
    var doughnut = new Chart(ctx, {
        type: 'doughnut',
        data: data,
        options: options
    });

    // Update chart with lecturer data and append lecturer list
    function updateChart() {
        var totalAvailable = lecturerPercentages.reduce((acc, curr) => acc + curr, 0);
        var totalUnavailable = 100 - totalAvailable;
        data.datasets[0].data = [totalAvailable, totalUnavailable];
        doughnut.update();

        //Display lecturer names and percentages
        var lecturerList = document.createElement('ul');
        lecturerList.classList.add('list-unstyled', 'text-end');
        for (var i = 0; i < lecturerNames.length; i++) {
            var listItem = document.createElement('li');
            listItem.innerHTML = lecturerNames[i] + ': ' + lecturerPercentages[i] + '%';
            lecturerList.appendChild(listItem);
        }

        // Append lecturer list to the appropriate container
        var container = document.querySelector('.doughnut-chart-container');
        // Create a container for the lecturer list
        var lecturerListContainer = document.createElement('div'); 
        // Add class to the container
        lecturerListContainer.classList.add('lecturer-list'); 
        // Append the lecturer list container alongside the doughnut chart container
        container.parentNode.insertBefore(lecturerListContainer, container.nextSibling);
        // Append lecturer list to the new container
        lecturerListContainer.appendChild(lecturerList);
         
    }

    //Update chart when document is loaded
    updateChart();

});

