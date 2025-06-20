function loadAnalytics() {
  fetch('/api/analytics/summary', {
    headers: { 'Authorization': 'Bearer ' + accessToken }
  })
  .then(res => res.json())
  .then(kpis => {
    const kpiDiv = document.getElementById('kpis');
    kpiDiv.innerHTML = `
      <p>Total: ${kpis.total}</p>
      <p>Completed: ${kpis.completed}</p>
      <p>Remaining: ${kpis.remaining}</p>`;
  });

  fetch('/api/analytics/by-date', {
    headers: { 'Authorization': 'Bearer ' + accessToken }
  })
  .then(res => res.json())
  .then(data => {
    const ctx = document.getElementById('todoChart').getContext('2d');
    new Chart(ctx, {
      type: 'bar',
      data: {
        labels: data.map(d => d.date),
        datasets: [{
          label: 'Todos by Day',
          data: data.map(d => d.count),
          backgroundColor: 'rgba(54, 162, 235, 0.6)'
        }]
      }
    });
  });
}