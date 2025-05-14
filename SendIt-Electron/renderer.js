// renderer.js
document.addEventListener('DOMContentLoaded', () => {
    const themeCssLink = document.getElementById('theme-css');
    const settingsBtn = document.getElementById('settings-btn');
    
    // Open settings window
    settingsBtn.addEventListener('click', () => {
      window.api.openSettings();
    });
    
    // Listen for theme updates
    window.api.onThemeUpdate((theme) => {
      themeCssLink.href = `css/${theme}.css`;
    });
  });