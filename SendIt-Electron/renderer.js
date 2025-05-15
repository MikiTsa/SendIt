// renderer.js
document.addEventListener('DOMContentLoaded', () => {
  const themeCssLink = document.getElementById('theme-css');
  const settingsBtn = document.getElementById('settings-btn');
  const loadDataBtn = document.getElementById('load-data-btn');
  
  // DOM elements for contact display
  const avatarElement = document.getElementById('avatar');
  const nicknameElement = document.getElementById('nickname');
  const statusCircleElement = document.getElementById('status-circle');
  const statusTextElement = document.getElementById('status-text');
  const emailElement = document.getElementById('email');
  const phoneElement = document.getElementById('phone');
  const lastSeenElement = document.getElementById('last-seen');
  
  // Open settings window
  settingsBtn.addEventListener('click', () => {
    window.api.openSettings();
  });
  
  // Load contact data
  loadDataBtn.addEventListener('click', () => {
    window.api.loadContactData();
  });
  
  // Listen for theme updates
  window.api.onThemeUpdate((theme) => {
    themeCssLink.href = `css/${theme}.css`;
  });
  
  // Listen for contact data updates
  window.api.onContactDataUpdate((contactData) => {
    // Update the UI with contact data
    nicknameElement.textContent = contactData.nickname;
    
    // Update status
    statusTextElement.textContent = contactData.status;
    statusCircleElement.className = ''; // Clear existing classes
    statusCircleElement.classList.add(`status-${contactData.status.toLowerCase()}`);
    
    // Update other details
    emailElement.textContent = contactData.email;
    phoneElement.textContent = contactData.phoneNumber;
    
    // Format date
    const lastSeenDate = new Date(contactData.lastSeen);
    lastSeenElement.textContent = lastSeenDate.toLocaleString();
    
    // Update avatar
    avatarElement.src = contactData.avatar;
  });
});