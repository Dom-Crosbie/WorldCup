const fs = require('fs');
const path = require('path');

function readState(statePath) {
  if (!fs.existsSync(statePath)) return { provisioned: [] };
  return JSON.parse(fs.readFileSync(statePath, 'utf-8'));
}

function writeState(statePath, state) {
  fs.mkdirSync(path.dirname(statePath), { recursive: true });
  fs.writeFileSync(statePath, JSON.stringify(state, null, 2));
}

module.exports = { readState, writeState };
