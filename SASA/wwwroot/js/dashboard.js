(function () {
    const init = window.__dashboardInit || {};

    function getOr(arr, i, def) { return (arr && arr.length > i) ? arr[i] : def; }

    const estadoChart = new Chart(document.getElementById('estadoChart'), {
        type: 'doughnut',
        data: {
            labels: ['Abiertos', 'En Proceso', 'Resueltos', 'Cancelados', 'En Espera'],
            datasets: [{
                data: [init.Abiertos || 0, init.EnProgreso || 0, init.Resueltos || 0, init.Cancelados || 0, init.EnEsperaDelUsuario || 0],
                backgroundColor: ['#012473', '#0347b8', '#28a745', '#d4a147', '#6c757d']
            }]
        },
        options: {
            plugins: {
                legend: { position: 'bottom' },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            var idx = context.dataIndex;
                            var count = context.dataset.data[idx];
                            return count + ' tiquete(s)';
                        }
                    }
                }
            }
        }
    });

    const rawSubcategoryLabels = init.SubcategoryLabels || [];
    const rawSubcategoryCounts = init.SubcategoryCounts || [];
    const rawSubcategoryDisplayLabels = init.SubcategoryDisplayLabels || [];
    const rawSubcategoryTicketCounts = init.SubcategoryTicketCounts || [];

    function makeColorsForNames(names) {
        var palette = ['#012473', '#0347b8', '#0d6efd', '#6610f2', '#6f42c1', '#198754', '#ffc107', '#fd7e14', '#dc3545', '#6c757d'];
        return (names || []).map(function (n, i) {
            return palette[i % palette.length];
        });
    }

    const subcategoriaCtxEl = document.getElementById('subcategoriaChart');
    var initialSubColors = makeColorsForNames(rawSubcategoryLabels);
    const subcategoriaCtx = subcategoriaCtxEl ? subcategoriaCtxEl.getContext('2d') : null;
    var subcategoriaChart = null;
    if (subcategoriaCtx) {
        subcategoriaChart = new Chart(subcategoriaCtx, {
            type: 'bar',
            data: {
                labels: (rawSubcategoryDisplayLabels && rawSubcategoryDisplayLabels.length) ? rawSubcategoryDisplayLabels : rawSubcategoryLabels,
                datasets: [{ data: rawSubcategoryTicketCounts.length ? rawSubcategoryTicketCounts : rawSubcategoryCounts, backgroundColor: initialSubColors }]
            },
            options: {
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            title: function() { return ''; },
                            label: function (context) {
                                const idx = (context.dataIndex !== undefined) ? context.dataIndex : -1;
                                const tickets = (rawSubcategoryTicketCounts && rawSubcategoryTicketCounts.length > idx) ? rawSubcategoryTicketCounts[idx] : ((context.parsed && context.parsed.y !== undefined) ? context.parsed.y : context.raw);
                                return tickets + ' tiquete(s)';
                            }
                        }
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { callback: function() { return ''; } },
                        title: { display: true, text: 'Cantidad de tiquetes' }
                    }
                }
            }
        });
    }

    const trendCtx = document.getElementById('trendChart').getContext('2d');
    const trendChart = new Chart(trendCtx, {
        type: 'line',
        data: {
            labels: init.TrendLabels || [],
            datasets: [
                { label: 'Abiertos', data: init.TrendAbiertos || init.TrendCreados || [], borderColor: '#0d6efd', fill: false },
                { label: 'Resueltos', data: init.TrendResueltos || [], borderColor: '#198754', fill: false },
                { label: 'En Proceso', data: init.TrendEnProgreso || [], borderColor: '#ffc107', fill: false },
                { label: 'En espera', data: init.TrendEspera || [], borderColor: '#d4a147', fill: false },
                { label: 'Cancelados', data: init.TrendCancelados || [], borderColor: '#da0c00', fill: false }
            ]
        },
        options: {
            scales: {
                y: {
                    ticks: { callback: function() { return ''; } },
                    title: { display: true, text: 'Cantidad de tiquetes' }
                }
            }
        }
    });

    async function refreshDashboard() {
        try {
            const res = await fetch('/Home/GetDashboardJson');
            if (!res.ok) return;
            const data = await res.json();

            const abiertos = data.abiertos ?? data.Abiertos ?? 0;
            const enProgreso = data.enProgreso ?? data.EnProgreso ?? 0;
            const resueltos = data.resueltos ?? data.Resueltos ?? 0;
            const cancelados = data.cancelados ?? data.Cancelados ?? 0;
            const enEspera = data.enEspera ?? data.EnEspera ?? data.EnEsperaDelUsuario ?? data.EnEsperaDelUsuario ?? 0;

            const primaryEl = document.getElementById('cardAbiertos'); if (primaryEl) primaryEl.innerText = abiertos;
            const warnEl = document.getElementById('cardEnProgreso'); if (warnEl) warnEl.innerText = enProgreso;
            const successEl = document.getElementById('cardResueltos'); if (successEl) successEl.innerText = resueltos;
            const dangerEl = document.getElementById('cardCancelados'); if (dangerEl) dangerEl.innerText = cancelados;
            const esperaEl = document.getElementById('cardEnEspera'); if (esperaEl) esperaEl.innerText = enEspera;

            estadoChart.data.datasets[0].data = [abiertos, enProgreso, resueltos, cancelados, enEspera];
            estadoChart.update();

            const subLabels = data.subcategoryLabels ?? data.SubcategoryLabels ?? [];
            const subCounts = data.subcategoryCounts ?? data.SubcategoryCounts ?? [];
            const subDisplayLabels = data.subcategoryDisplayLabels ?? data.SubcategoryDisplayLabels ?? rawSubcategoryDisplayLabels ?? [];
            const subTicketCounts = data.subcategoryTicketCounts ?? data.SubcategoryTicketCounts ?? rawSubcategoryTicketCounts ?? [];

            function normalizeArray(arr, len) {
                arr = arr || [];
                const out = new Array(len);
                for (let i = 0; i < len; i++) out[i] = (i < arr.length && arr[i] != null) ? arr[i] : 0;
                return out;
            }

            if (subcategoriaChart) {
                subcategoriaChart.data.labels = subDisplayLabels.length ? subDisplayLabels : subLabels;
                const len = (subcategoriaChart.data.labels || []).length;
                subcategoriaChart.data.datasets[0].data = normalizeArray((subTicketCounts && subTicketCounts.length) ? subTicketCounts : subCounts, len);
                subcategoriaChart.data.datasets[0].backgroundColor = makeColorsForNames(subLabels);
                subcategoriaChart.update();
            }

            const trendLabels = data.trendLabels ?? data.TrendLabels ?? [];
            const trendAbiertos = data.trendAbiertos ?? data.TrendAbiertos ?? [];
            const trendResueltos = data.trendResueltos ?? data.TrendResueltos ?? [];
            const trendEnProgreso = data.trendEnProgreso ?? data.TrendEnProgreso ?? [];
            const trendEspera = data.trendEspera ?? data.TrendEspera ?? [];
            const trendCancelados = data.trendCancelados ?? data.TrendCancelados ?? [];

            // debug: log trend arrays to help diagnose missing series
            console.debug('trendLabels', trendLabels);
            console.debug('trendAbiertos', trendAbiertos);
            console.debug('trendResueltos', trendResueltos);
            console.debug('trendEnProgreso', trendEnProgreso);
            console.debug('trendEspera', trendEspera);
            console.debug('trendCancelados', trendCancelados);

            trendChart.data.labels = trendLabels;
            trendChart.data.datasets[0].data = trendAbiertos;
            trendChart.data.datasets[1].data = trendResueltos;
            trendChart.data.datasets[2].data = trendEnProgreso;
            trendChart.data.datasets[3].data = trendEspera;
            trendChart.data.datasets[4] && (trendChart.data.datasets[4].data = trendCancelados);
            trendChart.update();
        } catch (e) {
            console.error('Error refreshing dashboard', e);
        }
    }

    setInterval(refreshDashboard, 10000);

})();
