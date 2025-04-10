import React, { useState, useEffect } from "react";
import { Button, Modal, Form, Table, Spinner, Alert } from "react-bootstrap";
import { PitchTypeAPI } from "../../API";
import "../../CSS/Admin/ManagePitchTypeAdmin.css";

const ManagePitchType = () => {
  const [pitchTypes, setPitchTypes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({
    name: "",
    price: "",
    limitPerson: "",
  });
  const [editingId, setEditingId] = useState(null);
  const [uploadedImages, setUploadedImages] = useState([]);
  const [deleteImageIds, setDeleteImageIds] = useState([]);

  const fetchPitchTypes = async () => {
    try {
      const response = await PitchTypeAPI.GetAll();
      const pitchTypeList = Array.isArray(response)
        ? response
        : response.items || [];
      setPitchTypes(pitchTypeList);
    } catch (err) {
      console.error("Lỗi khi lấy danh sách loại sân:", err);
      setError("Không thể tải danh sách loại sân.");
    }
  };

  useEffect(() => {
    fetchPitchTypes();
  }, []);

  const handleInputChange = (e) => {
    const { name, value, files } = e.target;

    if (name === "imageFile") {
      const newFiles = Array.from(files);
      setUploadedImages(newFiles);
    } else {
      setFormData({ ...formData, [name]: value });
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const dataToSend = new FormData();
      dataToSend.append("name", formData.name);
      dataToSend.append("price", formData.price);
      dataToSend.append("limitPerson", formData.limitPerson);

      // Append ảnh mới
      uploadedImages.forEach((image) => {
        dataToSend.append("imageFile", image);
      });

      // Nếu có ảnh cần xóa
      deleteImageIds.forEach((id) => {
        dataToSend.append("DeleteImageIds", id);
      });

      if (editingId) {
        dataToSend.append("id", editingId);
        await PitchTypeAPI.UpdatePitchType(editingId, dataToSend);
      } else {
        await PitchTypeAPI.CreatePitchType(dataToSend);
      }

      setShowModal(false);
      fetchPitchTypes();
    } catch (err) {
      console.error("Lỗi khi lưu loại sân:", err);
      setError(err.message || "Không thể lưu loại sân.");
    } finally {
      setLoading(false);
      setFormData({ name: "", price: "", limitPerson: "" });
      setUploadedImages([]);
      setDeleteImageIds([]);
      setEditingId(null);
    }
  };

  const handleEdit = (pitchType) => {
    setEditingId(pitchType.id);
    setFormData({
      name: pitchType.name,
      price: pitchType.price,
      limitPerson: pitchType.limitPerson,
    });
    setUploadedImages([]);
    setDeleteImageIds([]);
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm("Bạn có chắc chắn muốn xóa loại sân này?")) {
      setLoading(true);
      try {
        await PitchTypeAPI.DeletePitchType(id);
        // Xóa khỏi danh sách hiện tại luôn
        setPitchTypes((prev) => prev.filter((p) => p.id !== id));
      } catch (err) {
        console.error("Lỗi khi xóa loại sân:", err);
        setError(err || "Không thể xóa loại sân. Có thể loại sân đang được sử dụng.");
      } finally {
        setLoading(false);
      }
    }
  };

  const handleRemoveImage = (imageId) => {
    setDeleteImageIds((prev) => [...prev, imageId]);
  };

  return (
    <div className="container mt-4">
      <h2>Quản lý loại sân</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      {loading && <Spinner animation="border" className="my-3" />}

      <Button onClick={() => setShowModal(true)} className="mb-3" variant="success">
        Thêm mới
      </Button>

      <Table striped bordered hover responsive className="text-center">
        <thead className="table-dark">
          <tr>
            <th>STT</th>
            <th>Tên</th>
            <th>Giá</th>
            <th>Số lượng người tối đa</th>
            <th>Hành động</th>
          </tr>
        </thead>
        <tbody>
          {pitchTypes.map((pitchType, index) => (
            <tr key={pitchType.id}>
              <td>{index + 1}</td>
              <td>{pitchType.name}</td>
              <td>{pitchType.price}</td>
              <td>{pitchType.limitPerson}</td>
              <td>
                <Button
                  variant="warning"
                  size="sm"
                  onClick={() => handleEdit(pitchType)}
                >
                  Sửa
                </Button>{" "}
                <Button
                  variant="danger"
                  size="sm"
                  onClick={() => handleDelete(pitchType.id)}
                >
                  Xóa
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>
            {editingId ? "Chỉnh sửa loại sân" : "Thêm loại sân mới"}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Tên loại sân</Form.Label>
              <Form.Control
                type="text"
                name="name"
                value={formData.name}
                onChange={handleInputChange}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Giá</Form.Label>
              <Form.Control
                type="number"
                name="price"
                value={formData.price}
                onChange={handleInputChange}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Số lượng người tối đa</Form.Label>
              <Form.Control
                type="number"
                name="limitPerson"
                value={formData.limitPerson}
                onChange={handleInputChange}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Hình ảnh</Form.Label>
              <Form.Control
                type="file"
                name="imageFile"
                onChange={handleInputChange}
                accept="image/*"
                multiple
              />
              {uploadedImages.length > 0 && (
                <div className="mt-3 uploaded-images-container">
                  {uploadedImages.map((img, index) => (
                    <div key={index} className="uploaded-image-item">
                      <i className="bi bi-image"></i> {img.name}
                      <button
                        type="button"
                        className="btn btn-sm ms-2"
                        onClick={() => {
                          const newImages = [...uploadedImages];
                          newImages.splice(index, 1);
                          setUploadedImages(newImages);
                        }}
                      >
                        ❌
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </Form.Group>

            <Button variant="primary" type="submit" disabled={loading}>
              {loading ? "Đang lưu..." : "Lưu"}
            </Button>
          </Form>
        </Modal.Body>
      </Modal>
    </div>
  );
};

export default ManagePitchType;
