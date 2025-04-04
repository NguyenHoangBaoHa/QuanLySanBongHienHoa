import React, { useEffect, useState } from "react";
import { PitchAPI, PitchTypeAPI } from "../../API";
import { Table, Button, Spinner, Alert, Modal, Form } from "react-bootstrap";

const ManagePitchesAdmin = () => {
  const [pitches, setPitches] = useState([]);
  const [pitchTypes, setPitchTypes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // State cho Modal (Thêm/Cập nhật)
  const [showModal, setShowModal] = useState(false);
  const [editMode, setEditMode] = useState(false);
  const [formData, setFormData] = useState({
    id: null,
    name: "",
    idPitchType: "",
  });

  // Lấy danh sách sân và loại sân
  const fetchData = async () => {
    try {
      setLoading(true);
      const [pitchResponse, pitchTypeResponse] = await Promise.all([
        PitchAPI.GetAllPitches(),
        PitchTypeAPI.GetAll(),
      ]);
      setPitches(Array.isArray(pitchResponse) ? pitchResponse : []);
      setPitchTypes(Array.isArray(pitchTypeResponse) ? pitchTypeResponse : []);
    } catch (error) {
      setError("Không thể tải dữ liệu. Vui lòng thử lại!");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  // Xử lý hiển thị Modal
  const handleShowModal = (pitch = null) => {
    if (pitch) {
      // Chế độ cập nhật
      setEditMode(true);
      setFormData({
        id: pitch.id,
        name: pitch.name,
        idPitchType: pitch.idPitchType.toString(),
      });
    } else {
      // Chế độ thêm mới
      setEditMode(false);
      setFormData({ id: null, name: "", idPitchType: "" });
    }
    setShowModal(true);
  };

  const handleCloseModal = () => setShowModal(false);

  // Xử lý thay đổi form
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  // Xử lý thêm mới hoặc cập nhật sân
  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editMode) {
        await PitchAPI.UpdatePitch(formData.id, {
          name: formData.name,
          idPitchType: Number(formData.idPitchType),
        });
        alert("Cập nhật sân thành công!");
      } else {
        await PitchAPI.CreatePitch({
          name: formData.name,
          idPitchType: Number(formData.idPitchType),
        });
        alert("Thêm sân thành công!");
      }
      handleCloseModal();
      fetchData(); // Tải lại danh sách sân
    } catch (error) {
      alert(`Lỗi: ${error}`);
    }
  };

  // Xử lý xóa sân
  const handleDeletePitch = async (id) => {
    if (window.confirm("Bạn có chắc chắn muốn xóa sân này không?")) {
      try {
        await PitchAPI.DeletePitch(id);
        alert("Xóa sân thành công!");
        fetchData(); // Tải lại danh sách sân
      } catch (error) {
        alert("Lỗi khi xóa sân: " + error);
      }
    }
  };

  // Hiển thị trạng thái loading
  if (loading) {
    return (
      <div className="text-center mt-4">
        <Spinner animation="border" />
        <p>Đang tải dữ liệu...</p>
      </div>
    );
  }

  // Hiển thị thông báo lỗi
  if (error) {
    return <Alert variant="danger">{error}</Alert>;
  }

  return (
    <div>
      <h2 className="my-4">Quản lý sân</h2>

      {/* Nút thêm sân */}
      <Button variant="success" className="mb-3" onClick={() => handleShowModal()}>
        + Thêm sân
      </Button>

      {/* Bảng hiển thị danh sách sân */}
      <Table striped bordered hover responsive className="text-center">
        <thead className="table-dark">
          <tr>
            <th>#</th>
            <th>Tên Sân</th>
            <th>Tên Loại Sân</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {pitches.length > 0 ? (
            pitches.map((pitch, index) => (
              <tr key={pitch.id}>
                <td>{index + 1}</td>
                <td>{pitch.name}</td>
                <td>{pitch.pitchTypeName}</td>
                <td>
                  <Button
                    variant="warning"
                    className="me-2"
                    onClick={() => handleShowModal(pitch)}
                  >
                    Sửa
                  </Button>
                  <Button
                    variant="danger"
                    onClick={() => handleDeletePitch(pitch.id)}
                  >
                    Xóa
                  </Button>
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="4" className="text-center">
                Không có sân nào.
              </td>
            </tr>
          )}
        </tbody>
      </Table>

      {/* Modal thêm/cập nhật sân */}
      <Modal show={showModal} onHide={handleCloseModal} centered>
        <Modal.Header closeButton>
          <Modal.Title>{editMode ? "Cập nhật sân" : "Thêm sân mới"}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Tên sân</Form.Label>
              <Form.Control
                type="text"
                name="name"
                value={formData.name}
                onChange={handleChange}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Loại sân</Form.Label>
              <Form.Select
                name="idPitchType"
                value={formData.idPitchType}
                onChange={handleChange}
                required
              >
                <option value="">-- Chọn loại sân --</option>
                {pitchTypes.map((type) => (
                  <option key={type.id} value={type.id}>
                    {type.name}
                  </option>
                ))}
              </Form.Select>
            </Form.Group>

            <Button variant="primary" type="submit">
              {editMode ? "Cập nhật" : "Thêm mới"}
            </Button>
          </Form>
        </Modal.Body>
      </Modal>
    </div>
  );
};

export default ManagePitchesAdmin;
