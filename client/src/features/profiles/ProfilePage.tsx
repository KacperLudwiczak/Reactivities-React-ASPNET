import { observer } from "mobx-react-lite";
import { Grid } from "semantic-ui-react";
import ProfileHeader from "./ProfileHeader";
import ProfileContent from "./ProfileContent";
import { useParams } from "react-router-dom";
import { useStore } from "../../app/stores/store";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { useEffect } from "react";

function ProfilePage() {
  const { username } = useParams();
  const { profileStore } = useStore();
  const { loadingProfile, loadProfile, profile } = profileStore;

  useEffect(() => {
    if (username) loadProfile(username);
  }, [loadProfile, username]);
  
  if (loadingProfile)
    return <LoadingComponent inverted content="Loading profile..." />;
  if (!profile) return <h2>Problem loading profile</h2>;

  return (
    <Grid>
      <Grid.Column width="16">
        <ProfileHeader />
        <ProfileContent />
      </Grid.Column>
    </Grid>
  );
}

const ObservedProfilePage = observer(ProfilePage);
export default ObservedProfilePage;
