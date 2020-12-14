import './echarts.min.js'

export function scatter_chart_init(scatter_chart_element, data) {
    // based on prepared DOM, initialize echarts instance
    var myChart = echarts.init(scatter_chart_element);

    // specify chart configuration item and data
    var option = {
        backgroundColor: new echarts.graphic.RadialGradient(0.3, 0.3, 0.8, [{
            offset: 0,
            color: '#f7f8fa'
        }]),
        title: {
            text: 'Poo Detections'
        },
        xAxis: {
            splitLine: {
                lineStyle: {
                    type: 'dashed'
                }
            },
            min: 0,
            max: 24
        },
        yAxis: {
            splitLine: {
                lineStyle: {
                    type: 'dashed'
                }
            },
            scale: true
        },
        series: [{
            name: 'time vs duration',
            data: data,
            type: 'scatter',
            symbolSize: function (data) {
                return data[1] / 10.;
            },
            emphasis: {
                label: {
                    show: true,
                    formatter: function (param) {
                        return `${param.data[2]} ${param.data[1]}s`;
                    },
                    position: 'top'
                }
            },
            itemStyle: {
                shadowBlur: 10,
                shadowColor: 'rgba(120, 36, 50, 0.5)',
                shadowOffsetY: 5,
                color: new echarts.graphic.RadialGradient(0.4, 0.3, 1, [{
                    offset: 0,
                    color: 'rgb(251, 118, 123)'
                }, {
                    offset: 1,
                    color: 'rgb(204, 46, 72)'
                }])
            }
        }]
    };

    // use configuration item and data specified to show chart
    myChart.setOption(option);
}