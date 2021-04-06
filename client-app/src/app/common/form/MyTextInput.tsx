import { useField } from 'formik';
import React from 'react';
import { Form, Label } from 'semantic-ui-react';

interface Props {
    placeholder: string;
    name: string;
    label?: string;
}

export default function MyTextInput (props: Props) {
    /*This will tie the data to the field input we are using*/
    const [field, meta] = useField(props.name);

    /*Doble !! converts object into a boolean (if it exists or it's undefined)*/
    return (
        <Form.Field error={meta.touched && !!meta.error}>
            <label>{props.label}</label>
            <input {...field} {...props}/>
            {meta.touched && meta.error ? (
                <Label basic color='red'>{meta.error}</Label>
            ) : null}
        </Form.Field>
    )
}