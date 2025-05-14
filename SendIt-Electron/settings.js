// settings.js
document.addEventListener('DOMContentLoaded', () => {
    const themeCssLink = document.getElementById('theme-css');
    const lightThemeBtn = document.getElementById('light-theme-btn');
    const darkThemeBtn = document.getElementById('dark-theme-btn');
    
    lightThemeBtn.addEventListener('click', () => {
      window.api.changeTheme('light');
    });
    
    darkThemeBtn.addEventListener('click', () => {
      window.api.changeTheme('dark');
    });
    
    // Listen for theme updates
    window.api.onThemeUpdate((theme) => {
      themeCssLink.href = `css/${theme}.css`;
      
      // Update active button
      if (theme === 'light') {
        lightThemeBtn.classList.add('active');
        darkThemeBtn.classList.remove('active');
      } else {
        darkThemeBtn.classList.add('active');
        lightThemeBtn.classList.remove('active');
      }
    });
  });