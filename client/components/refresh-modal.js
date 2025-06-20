let refreshTimer;
function startRefreshCountdown() {
  clearInterval(refreshTimer);
  let counter = 60;
  const modal = document.getElementById('refresh-modal');
  const countdown = document.getElementById('countdown');
  modal.style.display = 'flex';

  refreshTimer = setInterval(() => {
    counter--;
    countdown.textContent = counter;
    if (counter <= 0) {
      clearInterval(refreshTimer);
      window.location.reload();
    }
  }, 1000);
}
