import React, { useEffect, useState } from 'react';
import { Table, Button, Modal, Form, Spinner, Alert } from 'react-bootstrap';
import { PitchAPI, PitchTypeAPI } from '../../API';

const ManagePitchesAdmin = () => {
  const [pitches, setPitches] = useState([]);
  const [pitchTypes, setPitchTypes] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({
    id: null,
    name: '',
    idPitchType: '',
    status: 0, // Mặc định là Available
  });
  const [isEdit, setIsEdit] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  // Mapping trạng thái từ Enum
  const pitchStatusEnum = {
    0: 'Trống',
    1: 'Đã Đặt',
    2: 'Bảo Trì',
  };

  // Chỉ hiển thị trạng thái mà Admin/Staff có thể chỉnh sửa
  const editableStatus = [
    { value: 0, label: 'Trống' },
    { value: 2, label: 'Bảo Trì' },
  ];

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    try {
      const pitchData = await PitchAPI.GetAllPitches();
      const pitchTypeData = await PitchTypeAPI.GetAll();
      setPitches(pitchData);
      setPitchTypes(pitchTypeData);
    } catch (error) {
      setError('Lỗi khi tải dữ liệu');
    } finally {
      setLoading(false);
    }
  };

  const handleShow = (pitch = null) => {
    if (pitch) {
      setIsEdit(true);
      setFormData({
        id: pitch.id,
        name: pitch.name,
        idPitchType: pitch.idPitchType || '',
        status: pitch.status,
      });
    } else {
      setIsEdit(false);
      setFormData({ id: null, name: '', idPitchType: '', status: 0 });
    }
    setShowModal(true);
  };

  const handleClose = () => setShowModal(false);

  const handleSave = async () => {
    if (!formData.idPitchType) {
      alert('Vui lòng chọn loại sân');
      return;
    }

    setLoading(true);
    try {
      if (formData.id) {
        await PitchAPI.UpdatePitch(formData.id, formData);
      } else {
        await PitchAPI.CreatePitch(formData);
      }
      loadData();
      handleClose();
    } catch (error) {
      setError('Lỗi khi lưu dữ liệu');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Bạn có chắc muốn xóa sân này không?')) {
      setLoading(true);
      try {
        await PitchAPI.DeletePitch(id);
        loadData();
      } catch (error) {
        setError('Lỗi khi xóa sân');
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <div className="container mt-4">
      <h2>Quản Lý Sân</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      <Button variant="primary" onClick={() => handleShow()}>Thêm Sân</Button>
      <Table striped bordered hover className="mt-3">
        <thead>
          <tr>
            <th>Tên Sân</th>
            <th>Loại Sân</th>
            <th>Trạng Thái</th>
            <th>Thao Tác</th>
          </tr>
        </thead>
        <tbody>
          {loading ? (
            <tr>
              <td colSpan="4" className="text-center"><Spinner animation="border" /></td>
            </tr>
          ) : (
            pitches.map((pitch) => (
              <tr key={pitch.id}>
                <td>{pitch.name}</td>
                <td>{pitchTypes.find(pt => pt.id === pitch.idPitchType)?.name}</td>
                <td>{pitchStatusEnum[pitch.status]}</td>
                <td>
                  <Button variant="warning" onClick={() => handleShow(pitch)}>Sửa</Button>{' '}
                  <Button variant="danger" onClick={() => handleDelete(pitch.id)}>Xóa</Button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </Table>

      <Modal show={showModal} onHide={handleClose}>
        <Modal.Header closeButton>
          <Modal.Title>{isEdit ? 'Sửa Sân' : 'Thêm Sân'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="formName">
              <Form.Label>Tên Sân</Form.Label>
              <Form.Control
                type="text"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              />
            </Form.Group>
            <Form.Group controlId="formPitchType">
              <Form.Label>Loại Sân</Form.Label>
              <Form.Control
                as="select"
                value={formData.idPitchType}
                onChange={(e) => setFormData({ ...formData, idPitchType: e.target.value })}
              >
                <option value="">Chọn Loại Sân</option>
                {pitchTypes.map((type) => (
                  <option key={type.id} value={type.id}>{type.name}</option>
                ))}
              </Form.Control>
            </Form.Group>
            <Form.Group controlId="formStatus">
              <Form.Label>Trạng Thái</Form.Label>
              <Form.Control
                as="select"
                value={formData.status}
                onChange={(e) => setFormData({ ...formData, status: e.target.value })}
              >
                {editableStatus.map((status) => (
                  <option key={status.value} value={status.value}>
                    {status.label}
                  </option>
                ))}
              </Form.Control>
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose}>Hủy</Button>
          <Button variant="primary" onClick={handleSave}>
            {loading ? <Spinner animation="border" size="sm" /> : 'Lưu'}
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default ManagePitchesAdmin;
