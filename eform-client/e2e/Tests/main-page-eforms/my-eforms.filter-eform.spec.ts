import loginPage from '../../Page objects/Login.page';
import myEformsPage from '../../Page objects/MyEforms.page';
import { generateRandmString } from '../../Helpers/helper-functions';

const expect = require('chai').expect;
const testTag1 = 'Test tag';
const testTag2 = 'Tag for test';
const countCreateEForm = 3;
let countRowsBeforeFiltering = 0;
const namesEForms = new Array<string>();
describe('My eforms', function () {
  before(async () => {
    await loginPage.open('/');
    await loginPage.login();
  });
  it('should be able to filter by 1 word in label input', async () => {
    await (await myEformsPage.idSortBtn()).click();
    const spinnerAnimation = $('#spinner-animation');
    await spinnerAnimation.waitForDisplayed({ timeout: 40000, reverse: true });
    for (let i = 0; i < countCreateEForm; i++) {
      const newEForm = generateRandmString();
      await myEformsPage.createNewEform(newEForm);
      namesEForms.push(newEForm);
    }
    const nameEFormForFiltering = namesEForms[1];
    countRowsBeforeFiltering = await myEformsPage.rowNum();
    expect(countRowsBeforeFiltering).equal(3);
    await myEformsPage.eformFilter.setValue(nameEFormForFiltering);
    await spinnerAnimation.waitForDisplayed({ timeout: 50000, reverse: true });
    await browser.pause(2000);
    const eform = myEformsPage.getEformsRowObjByNameEForm(
      nameEFormForFiltering
    );
    expect(eform).not.equal(null);
  });
  it('should be able to see all eforms by leaving label input empty', async () => {
    await myEformsPage.eformFilter.click();
    await browser.keys(['Control', 'a', 'Control', 'Delete']);
    await $('#spinner-animation').waitForDisplayed({ timeout: 50000, reverse: true });
    await browser.pause(2000);
    expect(await myEformsPage.rowNum()).equal(3);
  });
  it('should be able to filter using 1 tag', async () => {
    await myEformsPage.createNewTags([testTag1, testTag2]);
    // await (await myEformsPage.getEformsRowObjByNameEForm(namesEForms[1])).addTag(testTag2);
    let form = await myEformsPage.getEformsRowObjByNameEForm(namesEForms[1]);
    await form.addTag(testTag2);
    form = await myEformsPage.getEformsRowObjByNameEForm(namesEForms[2]);
    await form.addTag(testTag1);
    await myEformsPage.enterTagFilter(testTag1);
    expect(form.eFormName).eq((await myEformsPage.getFirstMyEformsRowObj()).eFormName);
  });
  it('should be able to filter using several tags', async () => {
    await myEformsPage.enterTagFilter(testTag2);
    expect(await myEformsPage.rowNum()).eq(2);
  });
  after(async () => {
    await myEformsPage.eformFilter.click();
    await browser.keys(['Control', 'a', 'Control', 'Delete']);
    await browser.pause(500);
    let lastTag = await $('#tagSelector > div > div > .ng-value > span');
    await lastTag.click();
    lastTag = await $('#tagSelector > div > div > .ng-value > span');
    await lastTag.click();
    await myEformsPage.removeTags([testTag1, testTag2]);
    await browser.pause(500);
    for (let i = 0; i < namesEForms.length; i++) {
      const form = await myEformsPage.getEformsRowObjByNameEForm(namesEForms[i]);
      await form.deleteEForm();
      await browser.pause(500);
    }
  });
});
