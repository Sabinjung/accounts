import http from '../httpService';

import AttachmentModel from '../../models/Timesheet/attachmentModel';

class ProjectService {
  public async uploadAttachment(projectId: number, file: File): Promise<boolean> {
    const formData = new FormData();
    formData.append('file', file);
    let result = await http.post('api/services/app/Project/UploadAttachment', formData, {
      params: { projectId },
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return result.data.result;
  }

  public async getAttachments(projectId: number): Promise<Array<any>> {
    let response = await http.get('api/services/app/Project/GetAttachments', {
      params: { projectId },
    });
    return (response.data && response.data.result && response.data.result.map((x: any) => new AttachmentModel(x))) || [];
  }

  public async deleteAttachment(projectId: number, attachmentId: number): Promise<boolean> {
    let result = await http.delete('api/services/app/Project/deleteAttachment', {
      params: { attachmentId, projectId },
    });
    return result.data.result;
  }
}

export default new ProjectService();
