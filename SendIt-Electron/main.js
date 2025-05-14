const { app, BrowserWindow, ipcMain, Menu } = require('electron');
const path = require('path');
const fs = require('fs');

let mainWindow;
let settingsWindow;
let currentTheme = 'light';

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

app.whenReady().then(() => {
  createMainWindow();
  
  // IPC handlers
  ipcMain.on('open-settings', () => {
    createSettingsWindow();
  });
  
  ipcMain.on('change-theme', (event, theme) => {
    currentTheme = theme;
    // Send theme update to all windows
    mainWindow.webContents.send('theme-update', theme);
    if (settingsWindow) {
      settingsWindow.webContents.send('theme-update', theme);
    }
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', () => {
  if (BrowserWindow.getAllWindows().length === 0) {
    createMainWindow();
  }
});