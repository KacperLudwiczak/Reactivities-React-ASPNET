import { Form, Formik } from "formik";
import { observer } from "mobx-react-lite";
import { Button } from "semantic-ui-react";
import MyTextArea from "../../app/common/form/MyTextArea";
import MyTextInput from "../../app/common/form/MyTextInput";
import { useStore } from "../../app/stores/store";
import * as Yup from "yup";

interface Props {
  setEditMode: (editMode: boolean) => void;
}

function ProfileEditForm({ setEditMode }: Props) {
  const {
    profileStore: { profile, updateProfile },
  } = useStore();

  return (
    <Formik
      initialValues={{
        displayName: profile?.displayName,
        bio: profile?.bio || "",
      }}
      onSubmit={(values) => {
        updateProfile(values).then(() => {
          setEditMode(false);
        });
      }}
      validationSchema={Yup.object({
        displayName: Yup.string().required(),
      })}
    >
      {({ isSubmitting, isValid, dirty }) => (
        <Form className="ui form">
          <MyTextInput placeholder="Display Name" name="displayName" />
          <MyTextArea rows={3} placeholder="Add your bio" name="bio" />
          <Button
            type="submit"
            loading={isSubmitting}
            content="Update profile"
            floated="right"
            disabled={!isValid || !dirty}
            style={{
              backgroundColor: "#54c8ff",
              color: "#fff",
              borderRadius: "25px",
            }}
          />
        </Form>
      )}
    </Formik>
  );
}

const ObservedProfileEditForm = observer(ProfileEditForm);
export default ObservedProfileEditForm;
