/** @jsx jsx */
import { Component } from 'react';
import { Pagination } from 'antd';
import { jsx, css } from '@emotion/core';
// @ts-ignore
import FileViewer from 'react-file-viewer';
import AppConsts from '../../../lib/appconst';
export default class AttachmentViewer extends Component<any, any> {
  constructor(props: any) {
    super(props);
    this.state = {
      offset: 0,
    };
  }

  get attachmentId() {
    const { attachments } = this.props;
    const { offset } = this.state;
    if (attachments && attachments[offset]) {
      const attachment = attachments[offset];
      return attachment.id;
    }
    return 0;
  }

  handlePageClick = (data: any) => {
    let selected = data;
    const { attachments } = this.props;
    const offset = selected - 1;
    this.setState({ offset }, () => {
      if (attachments && attachments[offset]) {
        const attachment = attachments[offset];
        const { onAttachmentChange } = this.props;
        onAttachmentChange && onAttachmentChange(attachment);
      }
    });
  };

  componentDidMount() {
    const { attachments } = this.props;
    const { offset } = this.state;
    if (attachments && attachments[offset]) {
      const attachment = attachments[offset];
      const { onAttachmentChange } = this.props;
      onAttachmentChange && onAttachmentChange(attachment);
    }
  }

  componentWillReceiveProps(nextProps: any) {
    const { attachments } = nextProps;
    const { offset } = this.state;
    if (attachments && attachments[offset]) {
      const attachment = attachments[offset];
      const { onAttachmentChange } = this.props;
      onAttachmentChange && onAttachmentChange(attachment);
    }
  }

  render() {
    const { attachments } = this.props;
    const pageCount = (attachments && attachments.length) || 0;
    const { offset } = this.state;
    const selectedAttachment = attachments && attachments[offset];
    let attachmentUrl = 'http://s1.q4cdn.com/806093406/files/doc_downloads/test.pdf';
    let type = 'application/pdf';
    if (selectedAttachment) {
      const { id, contentType } = selectedAttachment;
      attachmentUrl = `${AppConsts.remoteServiceBaseUrl}/Attachment/Index/${id}`;
      type = contentType;
    }

    return (
      <div style={{ flex: 1, position: 'relative', height: '80vh' }}>
        {type === 'application/pdf' ? (
          <object data={attachmentUrl} type={type} style={{ width: '100%', height: '100%' }}></object>
        ) : (
          <img src={attachmentUrl} style={{ maxWidth: '100%', height: 'auto' }} />
        )}
        <div
          css={css`
            margin-bottom: 15px;
            position: absolute;
            bottom: 0;
            left: 0;
            right: 0;
            display: flex;
            justify-content: center;
          `}
        >
          <Pagination defaultCurrent={1} total={pageCount} onChange={this.handlePageClick} defaultPageSize={1} />
        </div>
      </div>
    );
  }

  onError(e: any) {
    console.log(e);
  }
}
