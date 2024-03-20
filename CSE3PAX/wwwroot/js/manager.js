document.addEventListener('DOMContentLoaded', function () {

    console.log("Testing Javascript");

    // Variables
    var ctx = document.getElementById('availabilityChart').getContext('2d');
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

    // Update chart with lecturer data
    function updateChart() {
        var tableRows = document.querySelectorAll('.table tbody tr');
        var totalCurrentLoad = 0;
        var totalLecturers = tableRows.length;

        tableRows.forEach(function (row) {
            //Retrieve the current load and parse it as a floating-point number
            var currentLoad = parseFloat(row.querySelector('td:nth-child(4)').textContent);

            //Check if the parsed value is NaN, if so, treat it as 0
            if (isNaN(currentLoad)) {
                currentLoad = 0;
            }
            //Check correct values are being calculated
            console.log("Current load text: ", currentLoad)
            //Update total current load
            totalCurrentLoad = totalCurrentLoad + currentLoad;
        });

        var totalAvailable = ((totalLecturers * 6 - totalCurrentLoad) / (6 * totalLecturers)) * 100;
        var totalUnavailable = 100 - totalAvailable;

        data.datasets[0].data = [totalAvailable, totalUnavailable];
        doughnut.update();
    }

    //Update chart when document is loaded
    updateChart();

});

