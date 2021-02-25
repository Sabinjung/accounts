import React, { useEffect, useState } from 'react';
import { Button, Icon, Upload, Col, Row, message, Spin, Checkbox, Typography } from 'antd';
import { inject, observer } from 'mobx-react';
import Stores from '../../../stores/storeIdentifier';
import AttachmentViewer from '../../TimesheetReview/components/AttachmentViewer';
import AttachmentModel from '../../../models/Timesheet/attachmentModel';
import styled from '@emotion/styled';

const { Dragger } = Upload;
const { Text } = Typography;

const StyledCol = styled(Col)`
  .ant-upload-list {
    display: none;
  }
`;

export default inject(Stores.ProjectStore)(
  observer(
    ({
      visible,
      onClose,
      projectStore,
      projectId,
      bodyStyle,
      style,
      placement,
      className,
      onAttachmentSelected,
      enableTimesheetAttachment = false,
      shouldLoad = true,
    }: any) => {
      const [isLoading, setLoading] = useState(false);
      const [currentAttachment, setCurrentAttachment] = useState<AttachmentModel>();
      useEffect(() => {
        if (projectId && shouldLoad) {
          projectStore.getAttachments(projectId);
        }
      }, [projectId]);

      const uploadProps: any = {
        accept: 'image/*,.pdf',
        multiple: true,
        customRequest: async (args: any) => {
          setLoading(true);
          await projectStore.uploadAttachment(projectId, args.file);
          message.info('Attachment successfully uploaded');
          args.onSuccess();
          setLoading(false);
        },
      };

      const viewerRef = React.createRef<AttachmentViewer>();

      async function deleteAttachment() {
        if (viewerRef.current) {
          const { attachmentId } = viewerRef.current;
          if (attachmentId) {
            setLoading(true);
            await projectStore.deleteAttachment(projectId, attachmentId);
            message.info('Attachment successfully deleted');
            setLoading(false);
          }
        }
      }

      function onAttachmentSelect(e: any) {
        if (currentAttachment) {
          currentAttachment.select(e.target.checked);
        }
      }

      function onAttachmentChanged(attachment: AttachmentModel) {
        setCurrentAttachment(attachment);
      }

      const isAttachmentSelected: boolean = (currentAttachment && currentAttachment.isSelected) || false;

      return (
        <Row type="flex" style={{ flexDirection: 'column', height: '100%' }}>
          <Col style={{ width: '100%' }}>
            <Row type="flex" justify="space-between" className="mb-10">
              <StyledCol span={10}>
                {!enableTimesheetAttachment && (
                  <Dragger {...uploadProps}>
                    <Spin spinning={isLoading}>
                      <Text type="secondary">
                        <Icon type="plus" style={{ fontSize: '18px' }} /> Click or drag file to this area to upload
                      </Text>
                    </Spin>
                  </Dragger>
                )}
              </StyledCol>

              <Col>
                {enableTimesheetAttachment && (
                  <Checkbox onChange={onAttachmentSelect} checked={isAttachmentSelected}>
                    Attach Attachment to Timesheet
                  </Checkbox>
                )}
                {!enableTimesheetAttachment && (
                  <Button type="danger" onClick={deleteAttachment} style={{ marginTop: '8px' }}>
                    <Icon type="delete" /> Delete
                  </Button>
                )}
              </Col>
            </Row>
          </Col>
          <Col style={{ flex: 1 }}>
            <AttachmentViewer attachments={projectStore.attachments} ref={viewerRef} onAttachmentChange={onAttachmentChanged} />
          </Col>
        </Row>
      );
    }
  )
);
