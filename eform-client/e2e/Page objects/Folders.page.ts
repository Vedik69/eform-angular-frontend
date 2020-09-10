import {PageWithNavbarPage} from './PageWithNavbar.page';
import myEformsPage from './MyEforms.page';
import {Guid} from 'guid-typescript';
import {expect} from 'chai';
import {DeviceUsersRowObject} from './DeviceUsers.page';

class FoldersPage extends PageWithNavbarPage {
  constructor() {
    super();
  }

  public get newFolderBtn() {
    return $('#newFolderBtn');
  }

  public get createNameInput() {
    return $('#name');
  }

  public get createDescriptionInput() {
    return $('#description');
  }

  public get saveCreateBtn() {
    return $('#folderSaveBtn');
  }

  public get cancelCreateBtn() {
    return $('#cancelCreateBtn');
  }

  public get saveDeleteBtn() {
    $('#saveDeleteBtn').waitForDisplayed({timeout: 20000});
    $('#saveDeleteBtn').waitForClickable({timeout: 20000});
    return $('#saveDeleteBtn');
  }

  public get cancelDeleteBtn() {
    $('#cancelDeleteBtn').waitForDisplayed({timeout: 20000});
    $('#cancelDeleteBtn').waitForClickable({timeout: 20000});
    return $('#cancelDeleteBtn');
  }

  public get editNameInput() {
    return $('#editNameInput');
  }

  public get editDescriptionInput() {
    return $('#editDescriptionInput');
  }

  public get saveEditBtn() {
    return $('#saveEditBtn');
  }

  public get cancelEditBtn() {
    return $('#cancelEditBtn');
  }

  public get rowNum(): number {
    browser.pause(500);
    return $$('#tableBody > tr').length;
  }

  public get rowNumParents(): number {
    browser.pause(500);
    return $$('#folderTreeName').length;
  }

  getFolder(num): FoldersRowObject {
    return new FoldersRowObject(num);
  }

  getFolderFromTree(num): FoldersTreeRowObject {
    return new FoldersTreeRowObject(num);
  }

  public createNewFolder(name: string, description: string) {
    this.newFolderBtn.click();
    $('#name').waitForDisplayed({timeout: 10000});
    this.createNameInput.setValue(name);
    this.createDescriptionInput.setValue(description);
    this.saveCreateBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    $('#newFolderBtn').waitForDisplayed({timeout: 20000});
  }

  public editFolder(folder: FoldersRowObject, name = '', description = '') {
    folder.editBtn.click();
    $('#editNameInput').waitForDisplayed({timeout: 20000});
    if (name != null) {
      this.editNameInput.click();
      this.editNameInput.clearValue();
      this.editNameInput.setValue(name);
    }
    if (description != null) {
      this.editDescriptionInput.click();
      this.editDescriptionInput.clearValue();
      this.editDescriptionInput.setValue(description);
    }
    this.saveEditBtn.click();
    $('#newFolderBtn').waitForDisplayed({timeout: 20000});
  }

  public createFolderChild(rowNum, name = '', description = '') {
    $$('#folderTreeName')[rowNum - 1].click();
    $$('#createFolderChildBtn')[0].waitForDisplayed({timeout: 10000});
    $$('#createFolderChildBtn')[0].click();
    $('#name').waitForDisplayed({timeout: 10000});
    this.createNameInput.setValue(name);
    this.createDescriptionInput.setValue(description);
    this.saveCreateBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  }

  public deleteFolderChild(parentRowNum: number, childRowToDelete: number) {
    $$('#folderTreeOpenClose')[parentRowNum - 1].waitForDisplayed({timeout: 10000});
    $$('#folderTreeOpenClose')[parentRowNum - 1].click();
    browser.pause(1000);
    $$('#folderTreeName')[childRowToDelete - 1].waitForDisplayed({timeout: 10000});
    $$('#folderTreeName')[childRowToDelete - 1].click();
    $('#deleteFolderTreeBtn').click();
    this.saveDeleteBtn.click();
  }

  public editFolderChild(folder: FoldersTreeRowObject, name = '', description = '') {
    folder.editTreeBtn.click();
    if (name != null) {
      this.editNameInput.waitForDisplayed({timeout: 20000});
      this.editNameInput.click();
      this.editNameInput.clearValue();
      this.editNameInput.setValue(name);
    }
    if (description != null) {
      this.editDescriptionInput.waitForDisplayed({timeout: 20000});
      this.editDescriptionInput.click();
      this.editDescriptionInput.clearValue();
      this.editDescriptionInput.setValue(description);
    }
    this.saveEditBtn.click();
    $('#newFolderBtn').waitForDisplayed({timeout: 20000});
  }

  public editFolderTree(folder: FoldersTreeRowObject, name = '', description = '') {
    folder.editTreeBtn.click();
    $('#editNameInput').waitForDisplayed({timeout: 20000});
    if (name != null) {
      this.editNameInput.click();
      this.editNameInput.clearValue();
      this.editNameInput.setValue(name);
    }
    if (description != null) {
      this.editDescriptionInput.click();
      this.editDescriptionInput.clearValue();
      this.editDescriptionInput.setValue(description);
    }
    this.saveEditBtn.click();
    $('#newFolderBtn').waitForDisplayed({timeout: 20000});
  }
}

const foldersPage = new FoldersPage();
export default foldersPage;

export class FoldersRowObject {
  constructor(rowNum) {
    if ($$('#folderId')[rowNum - 1]) {
      this.folderId = $$('#folderId')[rowNum - 1];
      try {
        this.name = $$('#folderName')[rowNum - 1].getText();
      } catch (e) {
      }
      try {
        this.description = $$('#folderDescription')[rowNum - 1].getText();
      } catch (e) {
      }
      this.editBtn = $$('#folderEditBtn')[rowNum - 1];
      this.deleteBtn = $$('#folderDeleteBtn')[rowNum - 1];
    }
  }

  folderId;
  name;
  description;
  editBtn;
  deleteBtn;
}

export class FoldersTreeRowObject {
  constructor(rowNum) {
    if ($$('#folderTreeId')[rowNum - 1]) {
      // this.folderTreeId = $$('#folderTreeId')[rowNum - 1];
      // console.log(this.folderTreeId);
      try {
        this.nameTree = $$('#folderTreeName')[rowNum - 1].getText();
      } catch (e) {
      }
      try {
        this.descriptionTree = $$('#folderTreeDescription')[rowNum - 1].getText();
      } catch (e) {
      }
      this.editTreeBtn = $('#editFolderTreeBtn');
      this.deleteTreeBtn = $('#deleteFolderTreeBtn');
    }
  }

  folderTreeId;
  nameTree;
  descriptionTree;
  editTreeBtn;
  deleteTreeBtn;
}
