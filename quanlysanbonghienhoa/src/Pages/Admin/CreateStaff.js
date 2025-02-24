import React, { useState } from "react";
import { Form, Button, Col, Row, Container } from "react-bootstrap";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { AccountAPI } from "../../API";
import "../../CSS/CreateStaff.css";

const CreateStaff = () => {
    const [formData, setFormData] = useState({
        email: "",
        password: "",
        displayName: "",
        dateOfBirth: null,
        cccd: "",
        gender: "Male",
        phoneNumber: "",
        address: "",
        startDate: new Date(),
    });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const handleDateChange = (field, date) => {
        setFormData({ ...formData, [field]: date });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const formattedData = {
                ...formData,
                dateOfBirth: formData.dateOfBirth
                    ? formData.dateOfBirth.toISOString().slice(0, 10)
                    : "",
                startDate: formData.startDate.toISOString().slice(0, 10),
            };

            await AccountAPI.createStaff(formattedData);
            alert("Staff account created successfully!");
            setFormData({
                email: "",
                password: "",
                displayName: "",
                dateOfBirth: null,
                cccd: "",
                gender: "Male",
                phoneNumber: "",
                address: "",
                startDate: new Date(),
            });
        } catch (error) {
            alert("Failed to create staff account: " + error.message);
        }
    };

    return (
        <Container className="mt-5">
            <div className="form-wrapper shadow p-4 rounded bg-light">
                <h2 className="mb-4 text-center">Register Staff Account</h2>
                <Form onSubmit={handleSubmit}>
                    <Row className="mb-3">
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>Email</Form.Label>
                                <Form.Control
                                    type="email"
                                    name="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    placeholder="Enter email"
                                    required
                                />
                            </Form.Group>
                        </Col>
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>Password</Form.Label>
                                <Form.Control
                                    type="password"
                                    name="password"
                                    value={formData.password}
                                    onChange={handleChange}
                                    placeholder="Enter password"
                                    required
                                />
                            </Form.Group>
                        </Col>
                    </Row>

                    <Row className="mb-3">
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>Display Name</Form.Label>
                                <Form.Control
                                    type="text"
                                    name="displayName"
                                    value={formData.displayName}
                                    onChange={handleChange}
                                    placeholder="Enter display name"
                                    required
                                />
                            </Form.Group>
                        </Col>
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>Date of Birth</Form.Label>
                                <DatePicker
                                    selected={formData.dateOfBirth}
                                    onChange={(date) => handleDateChange("dateOfBirth", date)}
                                    dateFormat="dd/MM/yyyy"
                                    placeholderText="Select date of birth"
                                    showYearDropdown
                                    scrollableYearDropdown
                                    className="form-control"
                                    required
                                />
                            </Form.Group>
                        </Col>
                    </Row>

                    <Row className="mb-3">
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>CCCD</Form.Label>
                                <Form.Control
                                    type="text"
                                    name="cccd"
                                    value={formData.cccd}
                                    onChange={handleChange}
                                    placeholder="Enter CCCD"
                                    required
                                />
                            </Form.Group>
                        </Col>
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>Gender</Form.Label>
                                <Form.Select
                                    name="gender"
                                    value={formData.gender}
                                    onChange={handleChange}
                                    required
                                >
                                    <option value="Male">Male</option>
                                    <option value="Female">Female</option>
                                </Form.Select>
                            </Form.Group>
                        </Col>
                    </Row>

                    <Row className="mb-3">
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>Phone Number</Form.Label>
                                <Form.Control
                                    type="text"
                                    name="phoneNumber"
                                    value={formData.phoneNumber}
                                    onChange={handleChange}
                                    placeholder="Enter phone number"
                                    required
                                />
                            </Form.Group>
                        </Col>
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>Address</Form.Label>
                                <Form.Control
                                    type="text"
                                    name="address"
                                    value={formData.address}
                                    onChange={handleChange}
                                    placeholder="Enter address"
                                    required
                                />
                            </Form.Group>
                        </Col>
                    </Row>

                    <Row className="mb-3">
                        <Col md={6}>
                            <Form.Group>
                                <Form.Label>Start Date</Form.Label>
                                <DatePicker
                                    selected={formData.startDate}
                                    onChange={(date) => handleDateChange("startDate", date)}
                                    dateFormat="dd/MM/yyyy"
                                    placeholderText="Select start date"
                                    className="form-control"
                                    required
                                />
                            </Form.Group>
                        </Col>
                    </Row>

                    <Button variant="primary" type="submit" className="w-100">
                        Register Staff
                    </Button>
                </Form>
            </div>
        </Container>
    );
}

export default CreateStaff;