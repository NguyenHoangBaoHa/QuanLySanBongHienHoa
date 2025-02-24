import "bootstrap/dist/css/bootstrap.min.css";
import { Routes, Route, Navigate } from "react-router-dom";
import NavigationBar from "./Components/Navbar";
import { AuthProvider } from "./Context/AuthContext";

import Home from "./Pages/Home";
import Login from "./Pages/Login";
import Register from "./Pages/Register";
import ManagePitchTypes from "./Pages/Admin/ManagePitchTypes";
import ManagePitchesAdmin from "./Pages/Admin/ManagePitchesAdmin";
import ManageBookingsAdmin from "./Pages/Admin/ManageBookingsAdmin";
import RevenueReport from "./Pages/Admin/RevenueReport";
import CreateStaff from "./Pages/Admin/CreateStaff";
import ManagePitchesStaff from "./Pages/Staff/ManagePitchesStaff";
import ManageBookingsStaff from "./Pages/Staff/ManageBookingsStaff";

const ProtectedRoute = ({ role, children }) => {
  const token = localStorage.getItem("token");
  const userRole = localStorage.getItem("role");

  if (!token) return <Navigate to="/login" />;
  if (role && userRole !== role) return <Navigate to="/" />;
  return children;
};

const App = () => {
  return (
    <AuthProvider>
      <NavigationBar />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* Role Admin */}
        <Route
          path="/manage-pitches-admin"
          element={
            <ProtectedRoute role="Admin">
              <ManagePitchesAdmin />
            </ProtectedRoute>
          }
        />
        <Route
          path="/manage-pitch-types-admin"
          element={
            <ProtectedRoute role="Admin">
              <ManagePitchTypes />
            </ProtectedRoute>
          }
        />
        <Route
          path="/manage-bookings-admin"
          element={
            <ProtectedRoute role="Admin">
              <ManageBookingsAdmin />
            </ProtectedRoute>
          }
        />
        <Route
          path="/revenue-report-admin"
          element={
            <ProtectedRoute role="Admin">
              <RevenueReport />
            </ProtectedRoute>
          }
        />
        <Route
          path="/create-staff"
          element={
            <ProtectedRoute role="Admin">
              <CreateStaff />
            </ProtectedRoute>
          }
        />

        {/* Role Staff */}
        <Route
          path="/manage-pitches-staff"
          element={
            <ProtectedRoute role="Staff">
              <ManagePitchesStaff />
            </ProtectedRoute>
          }
        >
        </Route>
        <Route
          path="/manage-bookings-staff"
          element={
            <ProtectedRoute role="Staff">
              <ManageBookingsStaff />
            </ProtectedRoute>
          }
        ></Route>
      </Routes>
    </AuthProvider>
  );
};

export default App;
