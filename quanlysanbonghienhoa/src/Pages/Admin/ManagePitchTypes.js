import React, { useState, useEffect } from "react";
import { Button, Modal, Form, Table, Spinner, Alert, Image } from "react-bootstrap";
import { PitchTypeAPI } from "../../API";
import "../../CSS/Admin/ManagePitchTypeAdmin.css";

const ManagePitchType = () => {
  const [pitchTypes, setPitchTypes] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [formData, setFormData] = useState({ name: "", price: "", limitPerson: "", images: [] });
  const [editingId, setEditingId] = useState(null);
  const [uploadedImages, setUploadedImages] = useState([]);

  const fetchPitchTypes = async () => {
    setLoading(true);
    try {
      const data = await PitchTypeAPI.GetAll();
      setPitchTypes(data);
    } catch (err) {
      setError(err.message || "Không thể lấy danh sách loại sân.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPitchTypes();
  }, []);

  const handleInputChange = (e) => {
    const { name, value, files } = e.target;
    if (name === "images") {
      const newImages = files ? Array.from(files) : [];
      setFormData({ ...formData, images: newImages });
      setUploadedImages(newImages.map(file => URL.createObjectURL(file)));
    } else {
      setFormData({ ...formData, [name]: value });
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      if (editingId) {
        await PitchTypeAPI.UpdatePitchType(editingId, formData);
      } else {
        await PitchTypeAPI.CreatePitchType(formData);
      }
      setShowModal(false);
      fetchPitchTypes();
    } catch (err) {
      setError(err.message || "Không thể lưu loại sân.");
    } finally {
      setLoading(false);
      setFormData({ name: "", price: "", limitPerson: "", images: [] });
      setUploadedImages([]);
      setEditingId(null);
    }
  };

  const handleEdit = (pitchType) => {
    setEditingId(pitchType.id);
    setFormData({
      name: pitchType.name,
      price: pitchType.price,
      limitPerson: pitchType.limitPerson,
      images: [],
    });
    setUploadedImages(pitchType.images || []);
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (window.confirm("Bạn có chắc chắn muốn xóa loại sân này?")) {
      setLoading(true);
      try {
        await PitchTypeAPI.DeletePitchType(id);
        fetchPitchTypes();
      } catch (err) {
        setError(err.message || "Không thể xóa loại sân.");
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <div className="container mt-4">
      <h2>Quản lý loại sân</h2>
      {error && <Alert variant="danger">{error}</Alert>}
      {loading && <Spinner animation="border" className="my-3" />}

      <Button onClick={() => setShowModal(true)} className="mb-3">
        Thêm mới
      </Button>

      <Table striped bordered hover responsive className="text-center">
        <thead className="table-dark">
          <tr>
            <th>STT</th>
            <th>Tên</th>
            <th>Giá</th>
            <th>Số lượng người tối đa</th>
            {/* <th>Hình ảnh</th> */}
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
              {/* <td>
                {pitchType.images && pitchType.images.length > 0 ? (
                  <div className="image-container">
                    {pitchType.images.map((img, i) => (
                      <Image key={i} src={img.url} thumbnail width={50} height={50} className="me-1" />
                    ))}
                  </div>
                ) : (
                  <span>Không có ảnh</span>
                )}
              </td> */}
              <td>
                <Button variant="warning" size="sm" onClick={() => handleEdit(pitchType)}>
                  Sửa
                </Button>{" "}
                <Button variant="danger" size="sm" onClick={() => handleDelete(pitchType.id)}>
                  Xóa
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>{editingId ? "Chỉnh sửa loại sân" : "Thêm loại sân mới"}</Modal.Title>
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
                name="images"
                onChange={handleInputChange}
                multiple
                accept="image/*"
              />
              <div className="mt-3">
                {uploadedImages.map((img, index) => (
                  <Image key={index} src={img} thumbnail width={50} height={50} className="me-1" />
                ))}
              </div>
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