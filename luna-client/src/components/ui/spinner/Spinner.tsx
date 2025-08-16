import React from "react";
import { Spinner as SpinnerComponent } from 'react-bootstrap';

export const Spinner: React.FC = () => {
	return (
		<SpinnerComponent animation="border" role="status"/>
	)
}