import { useEffect, useState } from "react";
import { Table, Button, Modal, Form } from 'react-bootstrap';
import { PitchAPI, PitchTypeAPI } from "../../API";

const ManagePitchesStaff = () => {
  const [pitches, setPitches] = useState([]);
  const [pitchTypes, setPitchTypes] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({ id: null, name: '', idPitchType: '', status: 'Trống' });
  const [isEdit, setIsEdit] = useState(false);
  // Load dữ liệu sân và loại sân
  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const pitchData = await PitchAPI.GetAllPitches();
      const pitchTypeData = await PitchTypeAPI.GetAll();

      console.log("Dữ liệu sân: ", pitchData);
      console.log("Dữ liệu loại sân: ", pitchTypeData);

      setPitches(pitchData);
      setPitchTypes(pitchTypeData);
    } catch (error) {
      console.error('Lỗi khi tải dữ liệu:', error);
    }
  };

  // Mở popup (thêm mới hoặc chỉnh sửa)
  const handleShow = (pitch = null) => {
    if (pitch) {
      setIsEdit(true);
      setFormData({
        ...pitch,
        idPitchType: pitch.pitchType ? pitch.pitchType.id : '' // Đảm bảo `idPitchType` được cập nhật đúng
      });
    } else {
      setIsEdit(false);
      setFormData({ id: null, name: '', idPitchType: '', status: 'Trống' });
    }
    setShowModal(true);
  };

  const handleClose = () => setShowModal(false);

  // Lưu dữ liệu sân
  const handleSave = async () => {
    if (!formData.idPitchType) {
      alert('Vui lòng chọn loại sân');
      return;
    }

    try {
      if (formData.id) {
        await PitchAPI.UpdatePitch(formData.id, formData);
      } else {
        await PitchAPI.CreatePitch(formData);
      }
      loadData(); // Refresh danh sách
      handleClose();
    } catch (error) {
      console.error('Lỗi khi lưu dữ liệu:', error);
    }
  };

  // Xóa sân
  const handleDelete = async (id) => {
    if (window.confirm('Bạn có chắc muốn xóa sân này không?')) {
      try {
        await PitchAPI.DeletePitch(id);
        loadData();
      } catch (error) {
        console.error('Lỗi khi xóa sân:', error);
      }
    }
  };

  return (
    <div className="container mt-4">
      <h2>Quản Lý Sân</h2>
      <Button variant="primary" onClick={() => handleShow()}>
        Thêm Sân Mới
      </Button>
      <Table striped bordered hover className="mt-3">
        <thead>
          <tr>
            <th>STT</th>
            <th>Tên Sân</th>
            <th>Loại Sân</th>
            <th>Trạng Thái</th>
            <th>Hoạt Động</th>
          </tr>
        </thead>
        <tbody>
          {pitches.map((pitch, index) => (
            <tr key={pitch.id}>
              <td>{index + 1}</td>
              <td>{pitch.name}</td>
              <td>{pitch.pitchTypeName || 'N/A'}</td>
              <td>{pitch.status}</td>
              <td>
                <Button variant="warning" size="sm" onClick={() => handleShow(pitch)}>
                  Sửa
                </Button>
                <Button variant="danger" size="sm" onClick={() => handleDelete(pitch.id)}>
                  Xóa
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      {/* Popup Modal */}
      <Modal show={showModal} onHide={handleClose}>
        <Modal.Header closeButton>
          <Modal.Title>{isEdit ? 'Cập Nhật Sân' : 'Thêm Sân Mới'}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="formName">
              <Form.Label>Tên Sân</Form.Label>
              <Form.Control
                type="text"
                placeholder="Nhập tên sân"
                value={formData.name}
                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              />
            </Form.Group>
            <Form.Group controlId="formPitchType">
              <Form.Label>Loại Sân</Form.Label>
              <Form.Control
                as="select"
                value={formData.idPitchType || ''}
                onChange={(e) => setFormData({ ...formData, idPitchType: e.target.value })}
              >
                <option value="">-- Chọn loại sân --</option>
                {pitchTypes.map((type) => (
                  <option key={type.id} value={type.id}>
                    {type.name}
                  </option>
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
                <option value="Trống">Trống</option>
                <option value="Đã Đặt">Đã Đặt</option>
              </Form.Control>
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose}>
            Đóng
          </Button>
          <Button variant="primary" onClick={handleSave}>
            Lưu
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
}

export default ManagePitchesStaff;