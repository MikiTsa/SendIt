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
    },
    
    // Function 4: Load contact data from file
    loadContactData: () => {
      ipcRenderer.send('load-contact-data');
    },
    
    // Function 5: Listen for contact data updates
    onContactDataUpdate: (callback) => {
      ipcRenderer.on('contact-data-update', (event, contactData) => {
        callback(contactData);
      });
    }
  }
);