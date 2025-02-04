const fs = require('fs');
const path = require('path');

const abstractions = path.join(__dirname, 'auto', 'autolmsclient-abstractions', 'index.ts');
const method = path.join(__dirname, 'auto', 'autolmsclient-module', 'index.ts');

const abstractionsData = fs.readFileSync(abstractions, 'utf8');

const interfaceMatches = abstractionsData.match(/export interface (\w+)/g) || [];
const classMatches = abstractionsData.match(/export class (\w+)/g) || [];

const interfaces = interfaceMatches.map(m => m.replace('export interface ', '').replace('{', ''));
const classes = classMatches.map(m => m.replace('export class ', '').replace('{', ''));

const importInterfaces = `import { ${interfaces.join(', ')} } from '../autolmsclient-abstractions';\n`;
const importClasses = `import { ${classes.join(', ')} } from '../autolmsclient-abstractions';\n`;

let content = fs.readFileSync(method, 'utf8');

content = content.replace(/import .* from '\.\.\/autolmsclient-abstractions';\n/g, '');

content = importInterfaces + importClasses + content;

fs.writeFileSync(method, content);

console.log('Imported interfaces and classes successfully');