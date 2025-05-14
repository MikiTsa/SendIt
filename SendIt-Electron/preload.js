const { contextBridge, ipcRenderer } = require('electron');

// Expose protected methods that allow the renderer process to use
// the ipcRenderer without exposing the entire object
contextBridge.exposeInMainWorld(
  'api', {
    // Function 1: Open settings window
    openSettings: () => {
      ipcRenderer.send('open-settings');
    },
    
    // Function 2: Change theme
    changeTheme: (theme) => {
      ipcRenderer.send('change-theme', theme);
    },
    
    // Function 3: Listen for theme updates
    onThemeUpdate: (callback) => {
      ipcRenderer.on('theme-update', (event, theme) => {
        callback(theme);
      });
    }
  }
);