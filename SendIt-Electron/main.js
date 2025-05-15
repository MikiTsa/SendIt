const { app, BrowserWindow, ipcMain, dialog } = require('electron');
const path = require('path');
const fs = require('fs');

let mainWindow;
let settingsWindow;
let currentTheme = 'light'; // Default theme
let lastLoadedContactPath = '';

// Path to store app settings
const userDataPath = app.getPath('userData');
const lastContactFilePath = path.join(userDataPath, 'last-contact.json');
const settingsFilePath = path.join(userDataPath, 'settings.json');

// Load app settings
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

// Save app settings
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
  
  // After window is loaded, apply the current theme
  mainWindow.webContents.on('did-finish-load', () => {
    mainWindow.webContents.send('theme-update', currentTheme);
    // Load last contact data after window is loaded
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
  
  // When settings window is ready, send the current theme
  settingsWindow.webContents.on('did-finish-load', () => {
    settingsWindow.webContents.send('theme-update', currentTheme);
  });
}

// Load last saved contact data
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

// Save last loaded contact data
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
  // Load settings before creating windows
  loadAppSettings();
  createMainWindow();
  
  // IPC handlers
  ipcMain.on('open-settings', () => {
    createSettingsWindow();
  });
  
  ipcMain.on('change-theme', (event, theme) => {
    currentTheme = theme;
    // Save the theme setting immediately when changed
    saveAppSettings();
    
    // Send theme update to all windows
    mainWindow.webContents.send('theme-update', theme);
    if (settingsWindow) {
      settingsWindow.webContents.send('theme-update', theme);
    }
  });
  
  // Handle loading contact data
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