const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld(
  'api', {
    openSettings: () => {
      ipcRenderer.send('open-settings');
    },
    
    changeTheme: (theme) => {
      ipcRenderer.send('change-theme', theme);
    },
    
    onThemeUpdate: (callback) => {
      ipcRenderer.on('theme-update', (event, theme) => {
        callback(theme);
      });
    },
    
    loadContactData: () => {
      ipcRenderer.send('load-contact-data');
    },
    
    onContactDataUpdate: (callback) => {
      ipcRenderer.on('contact-data-update', (event, contactData) => {
        callback(contactData);
      });
    }
  }
);