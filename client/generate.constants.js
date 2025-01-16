import { promises as fs } from 'fs';
import path from 'path';
import { Parser } from 'xml2js';

const resxFilePath = path.join(__dirname, 'src/display-text/resources.resx');
const outputFilePath = path.join(__dirname, 'src/constants/constants.ts');

const parser = new Parser();

const readResxFile = async (filePath) => {
  try {
    return await fs.readFile(filePath, 'utf8');
  } catch (error) {
    throw new Error(`Error reading resx file: ${error.message}`);
  }
};

const parseResxData = (data) => {
  return new Promise((resolve, reject) => {
    parser.parseString(data, (err, result) => {
      if (err) {
        reject(new Error(`Error parsing resx file: ${err.message}`));
      } else {
        resolve(result);
      }
    });
  });
};

const generateConstants = (result) => {
  return result.root.data.reduce((acc, item) => {
    const key = item.$.name.replace('display_text_', '')
      .split('_')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join('');
    const value = item.value[0];
    acc[key] = { name: item.$.name, value };
    return acc;
  }, {});
};

const createConstantsContent = (constants) => {
  return `export enum DisplayConstants {
    ${Object.entries(constants).map(([key, { name, value }]) => `  /** @constant ${name} */\n  ${key} = "${value}",`).join('\n')}
    }`;
};

const writeConstantsFile = async (filePath, content) => {
  try {
    await fs.writeFile(filePath, content, 'utf8');
    console.log('Constants file generated successfully.');
  } catch (error) {
    throw new Error(`Error writing constants file: ${error.message}`);
  }
};

const main = async () => {
  try {
    const data = await readResxFile(resxFilePath);
    const result = await parseResxData(data);
    const constants = generateConstants(result);
    const constantsContent = createConstantsContent(constants);
    await writeConstantsFile(outputFilePath, constantsContent);
  } catch (error) {
    console.error(error.message);
  }
};

main();