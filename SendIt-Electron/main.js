const { app, BrowserWindow, ipcMain, dialog } = require('electron');
const path = require('path');
const fs = require('fs');

let mainWindow;
let settingsWindow;
let currentTheme = 'light'; 
let lastLoadedContactPath = '';

const userDataPath = app.getPath('userData');
const lastContactFilePath = path.join(userDataPath, 'last-contact.json');
const settingsFilePath = path.join(userDataPath, 'settings.json');

function loadAppSettings() {
  try {
    if (fs.existsSync(settingsFilePath)) {
      const settings = JSON.parse(fs.readFileSync(settingsFilePath, 'utf8'));
      if (settings.theme) {
        currentTheme = settings.theme;
      }
    }
  } catch (error) {
    console.error('Error loading app settings:', error);
  }
}

function saveAppSettings() {
  try {
    const settings = {
      theme: currentTheme
    };
    fs.writeFileSync(settingsFilePath, JSON.stringify(settings));
  } catch (error) {
    console.error('Error saving app settings:', error);
  }
}

function createMainWindow() {
  mainWindow = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false
    }
  });

  mainWindow.loadFile('index.html');
  
  mainWindow.on('closed', () => {
    app.quit();
  });
  
  mainWindow.webContents.on('did-finish-load', () => {
    mainWindow.webContents.send('theme-update', currentTheme);
    loadLastContactData();
  });
}

function createSettingsWindow() {
  if (settingsWindow) {
    settingsWindow.focus();
    return;
  }

  settingsWindow = new BrowserWindow({
    width: 400,
    height: 300,
    parent: mainWindow,
    modal: false,
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      contextIsolation: true,
      nodeIntegration: false
    }
  });

  settingsWindow.loadFile('settings.html');
  
  settingsWindow.on('closed', () => {
    settingsWindow = null;
  });
  
  settingsWindow.webContents.on('did-finish-load', () => {
    settingsWindow.webContents.send('theme-update', currentTheme);
  });
}

function loadLastContactData() {
  try {
    if (fs.existsSync(lastContactFilePath)) {
      const lastContactData = JSON.parse(fs.readFileSync(lastContactFilePath, 'utf8'));
      
      if (lastContactData.path && fs.existsSync(lastContactData.path)) {
        lastLoadedContactPath = lastContactData.path;
        const contactData = JSON.parse(fs.readFileSync(lastContactData.path, 'utf8'));
        mainWindow.webContents.send('contact-data-update', contactData);
      }
    }
  } catch (error) {
    console.error('Error loading last contact data:', error);
  }
}

function saveLastContactData() {
  if (lastLoadedContactPath) {
    try {
      const lastContactData = { path: lastLoadedContactPath };
      fs.writeFileSync(lastContactFilePath, JSON.stringify(lastContactData));
    } catch (error) {
      console.error('Error saving last contact data:', error);
    }
  }
}

app.whenReady().then(() => {
  loadAppSettings();
  createMainWindow();
  
  ipcMain.on('open-settings', () => {
    createSettingsWindow();
  });
  
  ipcMain.on('change-theme', (event, theme) => {
    currentTheme = theme;
    saveAppSettings();
    
    mainWindow.webContents.send('theme-update', theme);
    if (settingsWindow) {
      settingsWindow.webContents.send('theme-update', theme);
    }
  });
  
  ipcMain.on('load-contact-data', async () => {
    const { canceled, filePaths } = await dialog.showOpenDialog(mainWindow, {
      title: 'Select Contact JSON File',
      properties: ['openFile'],
      filters: [
        { name: 'JSON Files', extensions: ['json'] }
      ]
    });
    
    if (!canceled && filePaths.length > 0) {
      try {
        const contactData = JSON.parse(fs.readFileSync(filePaths[0], 'utf8'));
        lastLoadedContactPath = filePaths[0];
        saveLastContactData();
        mainWindow.webContents.send('contact-data-update', contactData);
      } catch (error) {
        dialog.showErrorBox('Error Loading File', 'The selected file is not a valid contact JSON file.');
      }
    }
  });
});

app.on('window-all-closed', () => {
  saveLastContactData();
  saveAppSettings();
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('before-quit', () => {
  saveLastContactData();
  saveAppSettings();
});

app.on('activate', () => {
  if (BrowserWindow.getAllWindows().length === 0) {
    createMainWindow();
  }
});