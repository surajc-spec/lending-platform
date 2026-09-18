import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import AppLayout from "./layouts/AppLayout";
import ApplyLoan from "./pages/ApplyLoan";
import Applications from "./pages/Applications";
import Dashboard from "./pages/Dashboard";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<AppLayout />}>
                    <Route path="/" element={<ApplyLoan />} />
                    <Route path="/applications" element={<Applications />} />
                    <Route path="/dashboard" element={<Dashboard />} />

                    <Route
                        path="*"
                        element={<Navigate to="/" replace />}
                    />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;