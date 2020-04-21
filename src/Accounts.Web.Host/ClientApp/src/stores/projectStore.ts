import { action, runInAction, observable } from 'mobx';
import projectService from '../services/projects/projectService';
import _ from 'lodash';
class ProjectStore {
  @observable attachments!: Array<any>;

  @action
  public async uploadAttachment(projectId: number, file: File) {
    await projectService.uploadAttachment(projectId, file);
    this.getAttachments(projectId);
  }

  @action
  public async getAttachments(projectId: number, shouldMerge: boolean = false) {
    const result = await projectService.getAttachments(projectId);
    runInAction(() => {
      if (shouldMerge) {
        this.attachments = _.unionBy(this.attachments, result, x => x.id);
      } else {
        this.attachments = result;
      }
    });
  }

  @action
  public async deleteAttachment(projectId: number, attachmentId: number) {
    await projectService.deleteAttachment(projectId, attachmentId);
    this.getAttachments(projectId);
  }
}

export default ProjectStore;
