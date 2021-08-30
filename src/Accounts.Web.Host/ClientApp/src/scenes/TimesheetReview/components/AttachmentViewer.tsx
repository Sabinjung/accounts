/** @jsx jsx */
import React, { Component } from 'react';
import { Col, Pagination, Row, Button } from 'antd';
import { jsx, css } from '@emotion/core';
// @ts-ignore
import FileViewer from 'react-file-viewer';
import AppConsts from '../../../lib/appconst';
import styled from '@emotion/styled';

const StyledDiv = styled.div`
  flex: 1;
  position: relative;
  min-height: 80vh;

  .lastBtn {
    margin-right: 0px;
  }
`;

const StyledImageDiv = styled.div`
overflow: hidden;
justifyContent: center;
min-height: 60vh;
margin-top: 50px;
`;

const StyledButton = styled(Button)`
  margin-right: 10px;
`;
export default class AttachmentViewer extends Component<any, any> {
  initX = 0;
  initY = 0;
  lastX = 0;
  lastY = 0;
  constructor(props: any) {
    super(props);
    this.state = {
      offset: 0,
      x: 0,
      y: 0,
      zoom: 1,
      rotate: 0,
      moving: false,
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
        this.initX = 0;
        this.initY = 0;
        this.lastX = 0;
        this.lastY = 0;
        this.setState({ x: 0, y: 0, zoom: 1, rotate: 0, moving: false });
      }
    });
  };

  createTransform = (x: any, y: any, zoom: any, rotate: any) => `translate3d(${x}px,${y}px,0px) scale(${zoom}) rotate(${rotate}deg)`;

  applyZoom = (type: any) => {
    let zoomStep = 0.3;
    debugger;
    switch (type) {
      case 'in':
        this.setState({ zoom: this.state.zoom + zoomStep });
        break;
      case 'out':
        let newZoom = this.state.zoom - zoomStep;
        if (newZoom < 1) break;
        else if (newZoom === 1) this.setState({ x: 0, y: 0, zoom: 1 });
        else this.setState({ zoom: newZoom });
        break;
    }
  };

  applyRotate = (type: any) => {
    switch (type) {
      case 'cw':
        this.setState({ rotate: this.state.rotate + 90 });
        break;
      case 'acw':
        this.setState({ rotate: this.state.rotate - 90 });
        break;
    }
  };

  reset = (e: any) => {
    this.initX = 0;
    this.initY = 0;
    this.lastX = 0;
    this.lastY = 0;
    this.stopSideEffect(e);
    this.setState({ x: 0, y: 0, zoom: 1, rotate: 0 });
  };

  getXY(e: any) {
    let x = 0;
    let y = 0;
    if (e.touches && e.touches.length) {
      x = e.touches[0].pageX;
      y = e.touches[0].pageY;
    } else {
      x = e.pageX;
      y = e.pageY;
    }
    return { x, y };
  }

  startMove = (e: any) => {
    if (this.state.zoom > 1 || this.state.rotate != 0) {
      this.setState({ moving: true });
      let xy = this.getXY(e);
      this.initX = xy.x - this.lastX;
      this.initY = xy.y - this.lastY;
    }
  };
  duringMove = (e: any) => {
    if (this.state.moving) {
      let xy = this.getXY(e);
      this.lastX = xy.x - this.initX;
      this.lastY = xy.y - this.initY;
      this.setState({
        x: xy.x - this.initX,
        y: xy.y - this.initY,
      });
    }
  };
  endMove = (e: any) => this.setState({ moving: false });
  stopSideEffect = (e: any) => e.stopPropagation();

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
    const { offset, x, y, zoom, moving, rotate } = this.state;
    const selectedAttachment = attachments && attachments[offset];
    let attachmentUrl = 'http://s1.q4cdn.com/806093406/files/doc_downloads/test.pdf';
    let type = 'application/pdf';
    if (selectedAttachment) {
      const { id, contentType } = selectedAttachment;
      attachmentUrl = `${AppConsts.remoteServiceBaseUrl}/Attachment/Index/${id}`;
      type = contentType;
    }

    return (
      <StyledDiv>
        {type === 'application/pdf' ? (
          <object data={attachmentUrl} type={type} style={{ width: '100%', height: '79vh' }}></object>
        ) : (
          <React.Fragment>
            <Row justify="end" align="middle" type="flex">
              <Col>
                <StyledButton size="large" title="Zoom In" icon="zoom-in" onClick={() => this.applyZoom('in')} />
                <StyledButton
                  size="large"
                  title="Zoom Out"
                  disabled={zoom > 1 ? false : true}
                  icon="zoom-out"
                  onClick={() => this.applyZoom('out')}
                />
                <StyledButton size="large" title="Reset" disabled={zoom > 1 || rotate != 0 ? false : true} icon="sync" onClick={this.reset} />
                <StyledButton size="large" title="Rotate Left" icon="undo" onClick={() => this.applyRotate('acw')} />
                <StyledButton size="large" title="Rotate Right" icon="redo" onClick={() => this.applyRotate('cw')} />
                <a href={attachmentUrl} download>
                  <StyledButton className="lastBtn" size="large" title="Download" icon="download"></StyledButton>
                </a>
              </Col>
            </Row>
            <StyledImageDiv>
              <img
                draggable="false"
                style={{
                  transform: this.createTransform(x, y, zoom, rotate),
                  cursor: zoom > 1 ? 'grab' : 'unset',
                  transition: moving ? 'none' : 'all 0.1s',
                  marginLeft: 'auto',
                  marginRight: 'auto',
                  display: 'block',
                  width: '100%',
                }}
                src={attachmentUrl}
                onMouseDown={this.startMove}
                onTouchStart={this.startMove}
                onMouseMove={this.duringMove}
                onTouchMove={this.duringMove}
                onMouseUp={this.endMove}
                onMouseLeave={this.endMove}
                onTouchEnd={this.endMove}
              />
            </StyledImageDiv>
          </React.Fragment>
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
      </StyledDiv>
    );
  }

  onError(e: any) {
    console.log(e);
  }
}
