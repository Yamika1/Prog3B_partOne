const barChartOptions = {
    series: [
        {
            data: [10, 8, 6, 4, 2],
            name: 'Sensors'
        }
    ],


chart: {
    type: 'bar',
    background: 'transparent',
    height: 350,
    toolbar: {
        show: false
    }
},

colors: [
    '#2962ff',
    '#d50000',
    '#2e7d32',
    '#ff6d00',
    '#583cb3'
],

plotOptions: {
    bar: {
        distributed: true,
        borderRadius: 4,
        horizontal: false,
        columnWidth: '40%'
    }
},

dataLabels: {
    enabled: false
},

fill: {
    opacity: 1
},

grid: {
    borderColor: '#55596e',
    yaxis: {
        lines: {
            show: true
        }
    },
    xaxis: {
        lines: {
            show: true
        }
    }
},

legend: {
    labels: {
        colors: '#f5f7ff'
    },
    show: true,
    position: 'top'
},

stroke: {
    colors: ['transparent'],
    show: true,
    width: 2
},

tooltip: {
    shared: true,
    intersect: false,
    theme: 'dark'
},

xaxis: {
    categories: [
        'Temperature Sensors',
        'Pressure Sensors',
        'Touch Sensors',
        'Proximity and Motion Sensors'
    ],

    axisBorder: {
        show: true,
        color: '#55596e'
    },

    axisTicks: {
        show: true,
        color: '#55596e'
    },

    labels: {
        style: {
            colors: '#f5f7ff'
        }
    }
},

yaxis: {
    title: {
        text: 'Sensor Count',
        style: {
            color: '#f5f7ff'
        }
    },

    axisBorder: {
        color: '#55596e',
        show: true
    },

    axisTicks: {
        color: '#55596e',
        show: true
    },

    labels: {
        style: {
            colors: '#f5f7ff'
        }
    }
}


};

const barChartElement = document.querySelector('#bar-chart');

if (barChartElement) {
    const barChart = new ApexCharts(
        barChartElement,
        barChartOptions
    );

    
barChart.render();


}

const areaChartOptions = {
    series: [
        {
            name: 'Sensor Readings',
            data: [31, 40, 28, 51, 42, 109, 100]
        },

        
    {
        name: 'Alerts',
        data: [11, 32, 45, 32, 34, 52, 41]
    }
],

chart: {
    type: 'area',
    background: 'transparent',
    height: 350,
    stacked: false,

    toolbar: {
        show: false
    }
},

colors: [
    '#00ab57',
    '#d50000'
],

labels: [
    'Jan',
    'Feb',
    'Mar',
    'Apr',
    'May',
    'Jun',
    'Jul'
],

dataLabels: {
    enabled: false
},

fill: {
    gradient: {
        opacityFrom: 0.4,
        opacityTo: 0.1,
        shadeIntensity: 1,
        stops: [0, 100],
        type: 'vertical'
    },

    type: 'gradient'
},

grid: {
    borderColor: '#55596e',

    yaxis: {
        lines: {
            show: true
        }
    },

    xaxis: {
        lines: {
            show: true
        }
    }
},

legend: {
    labels: {
        colors: '#f5f7ff'
    },

    show: true,
    position: 'top'
},

markers: {
    size: 6,
    strokeColors: '#1b2635',
    strokeWidth: 3
},

stroke: {
    curve: 'smooth'
},

xaxis: {
    axisBorder: {
        color: '#55596e',
        show: true
    },

    axisTicks: {
        color: '#55596e',
        show: true
    },

    labels: {
        offsetY: 5,

        style: {
            colors: '#f5f7ff'
        }
    }
},

yaxis: {
    title: {
        text: 'Sensor Readings',

        style: {
            color: '#f5f7ff'
        }
    },

    labels: {
        style: {
            colors: ['#f5f7ff']
        }
    }
},

tooltip: {
    shared: true,
    intersect: false,
    theme: 'dark'
}


};


const areaChartElement = document.querySelector('#area-chart');

if (areaChartElement) {
    const areaChart = new ApexCharts(
        areaChartElement,
        areaChartOptions
    );

   
areaChart.render();


}
